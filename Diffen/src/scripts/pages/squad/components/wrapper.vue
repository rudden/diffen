<template>
	<div>
		<navbar />
		<div class="container pt-4 pb-5">
			<div class="row">
				<div class="col-lg-8 col-md-8 col-sm-12">
					<ul class="list-group media-list media-list-stream">
						<li class="list-group-item p-4">
							<modal v-bind="modalAttributes.newPlayer" v-if="loggedInUserIsAdmin">
								<template slot="body">
									<form-component :save="create" />
								</template>
							</modal>
							<h4 class="mb-0">Spelartruppen</h4>
						</li>
						<li class="list-group-item media">
							<template v-if="!loading">
								<table class="table table-sm mb-0">
									<thead class="thead-dark">
										<tr>
											<th scope="col">Namn</th>
											<th scope="col" class="d-none d-sm-table-cell">Tröjnummer</th>
											<th scope="col" class="d-none d-sm-table-cell">I antal startelvor</th>
											<th scope="col" class="d-none d-sm-table-cell">Antal positioner</th>
											<th scope="col" v-if="loggedInUserIsAdmin"></th>
										</tr>
									</thead>
									<tbody>
										<tr v-for="player in players" v-bind:key="player.id">
											<td>
												{{ player.fullName }}
												<span class="badge badge-primary ml-1" v-if="player.isCaptain">kapten</span>
												<span class="badge badge-danger ml-1" v-if="player.isHereOnLoan">inlånad</span>
												<span class="badge badge-warning ml-1" v-if="player.isOutOnLoan">utlånad</span>
												<span class="badge badge-danger ml-1" v-if="player.isSold">såld</span>
											</td>
											<td class="d-none d-sm-table-cell">{{ player.kitNumber == 0 ? '' : player.kitNumber }}</td>
											<td class="d-none d-sm-table-cell">{{ player.inNumberOfStartingElevens }}</td>
											<td class="d-none d-sm-table-cell">{{ player.availablePositions.length }}</td>
											<td v-if="loggedInUserIsAdmin">
												<modal v-bind="{ attributes: { name: `edit-${player.id}`, scrollable: true }, header: player.name, button: { icon: 'icon icon-edit' } }">
													<template slot="body">
														<form-component :player="player" :save="update" />
													</template>
												</modal>
											</td>
										</tr>
									</tbody>
								</table>
							</template>
							<template v-else>
								<loader v-bind="{ background: '#699ED0' }" />
							</template>
						</li>
					</ul>
				</div>
				<div class="col-lg-4 col-md-4 col-sm-12">
					<player-events />
				</div>
			</div>
		</div>
	</div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Action, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('squad', Getter)
const ModuleAction = namespace('squad', Action)

import { PageViewModel, Result, ResultType } from '../../../model/common'
import { Player, Position } from '../../../model/squad'
import { Player as CrudPlayer } from '../../../model/squad/crud'

import {
	GET_PLAYERS,
	FETCH_PLAYERS,
	FETCH_POSITIONS,
	CREATE_PLAYER,
	UPDATE_PLAYER
} from '../../../modules/squad/types'

import FormComponent from './form.vue'
import Modal from '../../../components/modal.vue'
import PlayerEvents from '../../../components/player-events.vue'

@Component({
	components: {
		Modal, FormComponent, PlayerEvents
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

	mounted() {
		Promise.all([this.loadPlayers(), this.loadPositions()])
			.then(() => this.loading = false)
	}

	get loggedInUserIsAdmin(): boolean {
        return this.vm.loggedInUser.inRoles.some(role => role == 'Admin')
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
}
</script>

<style lang="scss" scoped>
a {
	cursor: pointer;
}
</style>