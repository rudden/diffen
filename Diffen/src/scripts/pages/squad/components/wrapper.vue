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
						<li class="list-group-item media">
							<template v-if="!loading">
								<table-component :data="filteredPlayers" sort-by="data.numberOfPoints" sort-order="desc" @rowClick="rowClick">
									<table-column label="Efternamn" :hidden="true" show="lastName"></table-column>
									<table-column label="Namn" filter-on="fullName" sort-by="lastName" data-type="string">
										<template slot-scope="row">
											{{ row.fullName }}
											<span class="badge badge-warning ml-1" v-if="row.attributes.isOutOnLoan">utlånad</span>
											<span class="badge badge-danger ml-1" v-if="row.attributes.isSold">såld</span>
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
									<template slot="tfoot">
										<tr>
											<td colspan="11" style="border: none">
												<div class="form-check form-check-inline m-2">
													<input class="form-check-input" type="checkbox" id="includePlayersOutOnLoan" v-model="includePlayersOutOnLoan">
													<label class="form-check-label" for="includePlayersOutOnLoan">Inkludera utlånade spelare</label>
												</div>
											</td>
										</tr>
									</template>
								</table-component>
								<modal v-for="player in filteredPlayers" :key="player.id" v-bind="{ attributes: { name: `show-player-${player.id}`, scrollable: true }, header: 'Spelarinformation', button: { } }">
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
			<div class="row mt-3">
				<div class="col">
					<game-result-guess-league />
				</div>
			</div>
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
import { Player, Position, PlayerEvent, PlayerTableData } from '../../../model/squad'
import { Player as CrudPlayer } from '../../../model/squad/crud'

import {
	GET_PLAYERS,
	FETCH_PLAYERS,
	FETCH_POSITIONS,
	CREATE_PLAYER,
	UPDATE_PLAYER
} from '../../../modules/squad/types'

import GameResultGuessLeague from './game-result-guess-league.vue'
import PlayerComponent from './player.vue'
import Modal from '../../../components/modal.vue'
import PlayerEvents from '../../../components/player-events.vue'

interface IPlayerTableData {
	id: number
	fullName: string
	data: PlayerTableData
}

@Component({
	components: {
		Modal, PlayerComponent, PlayerEvents, GameResultGuessLeague
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

	filteredPlayers: Player[] = []

	includePlayersOutOnLoan: boolean = false

	$modal: any = (this as any).VModal

	mounted() {
		Promise.all([this.loadPlayers(), this.loadPositions()])
			.then(() => {
				this.filteredPlayers = this.players.filter((player: Player) => !player.attributes.isOutOnLoan)
				this.loading = false
			})
	}

	@Watch('includePlayersOutOnLoan')
		onChange() {
			this.filteredPlayers = this.includePlayersOutOnLoan ? this.players : this.players.filter((player: Player) => !player.attributes.isOutOnLoan)
		}

	get loggedInUserIsAdmin(): boolean {
        return this.vm.loggedInUser.inRoles.some(role => role == 'Admin' || role == 'GameAdmin')
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
		this.$modal.show(`show-player-${row.data.id}`)
	}
}
</script>

<style lang="scss" scoped>
a {
	cursor: pointer;
}
</style>