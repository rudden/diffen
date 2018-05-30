<template>
	<div>
		<navbar />
		<div class="container pt-4 pb-5">
			<div class="row">
				<div class="col">
					<ul class="list-group media-list media-list-stream">
						<li class="list-group-item p-4">
							<modal v-bind="modalAttributes.newPlayer" v-if="loggedInUserIsAdmin">
								<template slot="body">
									<player-component :save="create" />
								</template>
							</modal>
							<h4 class="mb-0">Spelartruppen</h4>
						</li>
						<li class="list-group-item media p-4">
							<div class="form-inline flow-root" style="width: 100%" :class="{ 'div-disabled': loading }">
								<input type="text" class="form-control" v-model="playerSearch" placeholder="Sök efter en spelare">
								<div class="input-group float-right">
									<v-multiselect v-model="gameType" track-by="name" label="name" placeholder="Välj en matchtyp" :options="gameTypes" :searchable="false" :allow-empty="false" selectLabel="" selectedLabel="Vald" deselectLabel="">
										<template slot="singleLabel" slot-scope="{ option }">Visar händelser ur: <strong>{{ option.name }}</strong></template>
									</v-multiselect>
								</div>
							</div>
						</li>
						<li class="list-group-item media p-4">
							<template v-if="!loading">
								<table-component :data="filteredPlayers" sort-by="data.numberOfPoints" sort-order="desc" @rowClick="rowClick" :show-filter="false">
									<table-column label="Efternamn" :hidden="true" show="player.lastName"></table-column>
									<table-column label="Namn" filter-on="player.fullName" sort-by="player.lastName" data-type="string">
										<template slot-scope="row">
											{{ row.player.fullName }}
											<span class="badge badge-warning ml-1" v-if="row.player.attributes.isOutOnLoan">utlånad</span>
											<span class="badge badge-danger ml-1" v-if="row.player.attributes.isSold">såld</span>
										</template>
									</table-column>
									<table-column label="Matcher" show="data.numberOfGames" data-type="numeric" :cell-class="'text-center'" :header-class="'text-center'"></table-column>
									<table-column label="Starter" show="data.numberOfGamesFromStart" data-type="numeric" :cell-class="'text-center'" :header-class="'text-center'"></table-column>
									<table-column label="Utbytt" show="data.numberOfGamesSubstituteOut" data-type="numeric" :cell-class="'text-center'" :header-class="'text-center'"></table-column>
									<table-column label="Inbytt" show="data.numberOfGamesSubstituteIn" data-type="numeric" :cell-class="'text-center'" :header-class="'text-center'"></table-column>
									<table-column label="Minuter" show="data.numberOfMinutesPlayed" data-type="numeric" :cell-class="'text-center'" :header-class="'text-center'"></table-column>
									<table-column label="Gula" show="data.numberOfYellowCards" data-type="numeric" :cell-class="'text-center'" :header-class="'text-center'"></table-column>
									<table-column label="Röda" show="data.numberOfRedCards" data-type="numeric" :cell-class="'text-center'" :header-class="'text-center'"></table-column>
									<table-column label="Mål" show="data.numberOfGoals" data-type="numeric" :cell-class="'text-center'" :header-class="'text-center'"></table-column>
									<table-column label="Assist" show="data.numberOfAssists" data-type="numeric" :cell-class="'text-center'" :header-class="'text-center'"></table-column>
									<table-column label="Poäng" show="data.numberOfPoints" data-type="numeric" :cell-class="'text-center'" :header-class="'text-center'"></table-column>
									<table-column label="Minut per poäng" show="data.numberOfMinutesPerPoint" data-type="numeric" :cell-class="'text-center'" :header-class="'text-center'"></table-column>
									<template slot="tfoot">
										<tr>
											<td colspan="12">
												<div class="form-check form-check-inline m-2">
													<input class="form-check-input" type="checkbox" id="includePlayersOutOnLoan" v-model="includePlayersOutOnLoan">
													<label class="form-check-label" for="includePlayersOutOnLoan">Inkludera utlånade spelare</label>
												</div>
											</td>
										</tr>
									</template>
								</table-component>
								<modal v-for="player in players" :key="player.id" v-bind="{ attributes: { name: `show-player-${player.id}`, scrollable: true }, header: 'Spelarinformation', button: { } }">
									<template slot="body">
										<player-component :player="player" :save="update" :editable="false" />
									</template>
								</modal>
							</template>
							<template v-else>
								<loader v-bind="{ background: '#699ED0' }" />
							</template>
						</li>
					</ul>
				</div>
			</div>
			<!-- <div class="row mt-3">
				<div class="col">
					<game-result-guess-league />
				</div>
			</div> -->
		</div>
	</div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component, Watch } from 'vue-property-decorator'
import { Getter, Action, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('squad', Getter)
const ModuleAction = namespace('squad', Action)

import { PageViewModel, Result, ResultType } from '../../../model/common'
import { Player, Position, PlayerEvent, PlayerTableData, Game, PlayerStatistics, PlayerToLineup, GameEventType, GameType, PlayerEventOnPlayer, PlayerAttributes } from '../../../model/squad'
import { Player as CrudPlayer } from '../../../model/squad/crud'

import {
	GET_PLAYERS,
	FETCH_PLAYERS,
	FETCH_POSITIONS,
	CREATE_PLAYER,
	UPDATE_PLAYER
} from '../../../modules/squad/types'

import PlayerComponent from './player.vue'
import Modal from '../../../components/modal.vue'
import PlayerEvents from '../../../components/player-events.vue'

interface IPlayer {
	id: number
	fullName: string
	lastName: string
	attributes: PlayerAttributes
}

interface IPlayerStatistics {
	player: IPlayer
	data: PlayerTableData
}

@Component({
	components: {
		Modal, PlayerComponent, PlayerEvents
	}
})
export default class Wrapper extends Vue {
	@State(state => state.vm) vm: PageViewModel
	@ModuleGetter(GET_PLAYERS) players: Player[]
	@ModuleAction(FETCH_PLAYERS) loadPlayers: () => Promise<void>
	@ModuleAction(FETCH_POSITIONS) loadPositions: () => Promise<void>
	@ModuleAction(UPDATE_PLAYER) updatePlayer: (payload: { player: CrudPlayer }) => Promise<Result[]>
	@ModuleAction(CREATE_PLAYER) createPlayer: (payload: { player: CrudPlayer }) => Promise<Result[]>

	loading: boolean = true

	modalAttributes: any = {
		newPlayer: {
			attributes: {
				name: 'new-player',
				scrollable: true
			},
			header: 'Ny spelare',
			button: {
				icon: 'icon icon-plus float-right',
				text: 'Skapa ny spelare'
			}
		}
	}

	includePlayersOutOnLoan: boolean = false

	gameType: any = { id: GameType.League, name: 'Allsvenskan' }
	
	gameTypes: any = [
		{ id: GameType.Cup, name: 'Cupen' },
		{ id: GameType.League, name: 'Allsvenskan' },
		{ id: GameType.Training, name: 'Träningsmatcher' },
		{ id: GameType.EuropaLeague, name: 'Europa league' },
		{ id: GameType.All, name: 'Alla matchtyper' }
	]

	playerSearch: string = ''

	$modal: any = (this as any).VModal

	mounted() {
		Promise.all([this.loadPlayers(), this.loadPositions()]).then(() => this.loading = false)
	}

	get loggedInUserIsAdmin(): boolean {
        return this.vm.loggedInUser.inRoles.some(role => role == 'Admin' || role == 'GameAdmin')
	}

	get filteredPlayers() {
		return this.includePlayersOutOnLoan 
			? this.statistics.filter((s: IPlayerStatistics) => {
				return s.player.fullName.toLowerCase().includes(this.playerSearch.toLowerCase())
			}) 
			: this.statistics.filter((s: IPlayerStatistics) => !s.player.attributes.isOutOnLoan).filter((s: IPlayerStatistics) => {
				return s.player.fullName.toLowerCase().includes(this.playerSearch.toLowerCase())
			})
	}

	get statistics() {
		return this.players.map((player: Player) => {
			return <IPlayerStatistics> {
				player: {
					id: player.id,
					fullName: player.fullName,
					lastName: player.lastName,
					attributes: player.attributes
				},
				data: this.calculate(player)
			}
		})
	}

	calculate(player: Player) {
		let data = new PlayerTableData()

		let gamesWithoutEvents = this.gameType.id !== GameType.All ? player.statistics.gamesWithoutEvents.filter((g: Game) => g.type == this.gameType.id) : player.statistics.gamesWithoutEvents
		let distinctGamesWithEvents = this.gameType.id !== GameType.All ? player.statistics.distinctGamesWithEvents.filter((g: Game) => g.type == this.gameType.id) : player.statistics.distinctGamesWithEvents
		let events = this.gameType.id !== GameType.All ? player.statistics.events.filter((e: PlayerEventOnPlayer) => e.gameType == this.gameType.id) : player.statistics.events

		if (gamesWithoutEvents.length > 0) {
			for (var game of gamesWithoutEvents) {
				data.numberOfGames++
				data.numberOfGamesFromStart++
				data.numberOfMinutesPlayed += 90
			}
		}

		if (player.statistics.events.length <= 0) {
			return data
		}

		let gamesFromStart: Game[] = distinctGamesWithEvents.filter((g: Game) => g.lineup && g.lineup.players.map((p: PlayerToLineup) => p.player.id).includes(player.id))

		let gamesFromStartSubstitutedOut = gamesFromStart.filter((g: Game) => g.playerEvents.filter((e: PlayerEvent) => e.eventType == GameEventType.SubstituteOut && e.player.id == player.id))
		let gamesFromStartNotSubstitutedOut = gamesFromStart.filter((g: Game) => !g.playerEvents.filter((e: PlayerEvent) => e.eventType == GameEventType.SubstituteOut && e.player.id == player.id).map((e: PlayerEvent) => e.player.id).includes(player.id))
		let gamesSubstitutedIn = distinctGamesWithEvents.filter((g: Game) => g.playerEvents.filter((e: PlayerEvent) => e.eventType == GameEventType.SubstituteIn && e.player.id == player.id))

		if (gamesFromStartNotSubstitutedOut.length > 0) {
			data.numberOfMinutesPlayed += 90 * gamesFromStart.length
		}
		if (gamesFromStartSubstitutedOut.length > 0) {
			for (var game of gamesFromStartSubstitutedOut) {
				let event: PlayerEvent = game.playerEvents.filter((e: PlayerEvent) => e.eventType == GameEventType.SubstituteOut && e.player.id == player.id)[0]
				if (event) {
					data.numberOfMinutesPlayed += event.inMinute
				}
			}
		}
		if (gamesSubstitutedIn.length > 0) {
			for (var game of gamesSubstitutedIn) {
				let event: PlayerEvent = game.playerEvents.filter((e: PlayerEvent) => e.eventType == GameEventType.SubstituteIn && e.player.id == player.id)[0]
				if (event) {
					data.numberOfMinutesPlayed += 90 + game.numberOfAddonMinutes - event.inMinute
				}
			}
		}

		data.numberOfGames += distinctGamesWithEvents.length
		data.numberOfGamesFromStart += gamesFromStart.length
		data.numberOfGamesSubstituteIn += events.filter((e: PlayerEventOnPlayer) => e.eventType == GameEventType.SubstituteIn).length
		data.numberOfGamesSubstituteOut += events.filter((e: PlayerEventOnPlayer) => e.eventType == GameEventType.SubstituteOut).length
		data.numberOfGoals += this.getNumberOfEvents(events, GameEventType.Goal)
		data.numberOfAssists += this.getNumberOfEvents(events, GameEventType.Assist)
		data.numberOfYellowCards += this.getNumberOfEvents(events, GameEventType.YellowCard)
		data.numberOfRedCards += this.getNumberOfEvents(events, GameEventType.RedCard)
		data.numberOfPoints += data.numberOfGoals + data.numberOfAssists
		let pointsPerPlayedMinute = data.numberOfMinutesPlayed / data.numberOfPoints
		if (!isNaN(pointsPerPlayedMinute) && isFinite(pointsPerPlayedMinute)) {
			data.numberOfMinutesPerPoint = Math.trunc(pointsPerPlayedMinute)
		}

		return data
	}

	getNumberOfEvents(events: PlayerEventOnPlayer[], eventType: GameEventType) {
		return events.filter((e: PlayerEventOnPlayer) => e.eventType == eventType).length
	}

	update(player: CrudPlayer) {
		return new Promise<Result[]>((resolve, reject) => {
			this.updatePlayer({ player: player }).then((results: Result[]) => resolve(results))
		})
	}

	create(player: CrudPlayer) {
		return new Promise<Result[]>((resolve, reject) => {
			this.createPlayer({ player: player }).then((results: Result[]) => resolve(results))
		})
	}

	rowClick(row: any) {
		this.$modal.show(`show-player-${row.data.player.id}`)
	}
}
</script>

<style lang="scss" scoped>
a {
	cursor: pointer;
}
@media (max-width: 768px) {
	.input-group.float-right {
		margin-top: 1.5rem;
	}
}
</style>