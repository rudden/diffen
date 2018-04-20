<template>
	<div class="container pt-4 pb-5">
		<div class="row">
			<div class="col-lg-12 col-md-12">
				<div class="card">
					<div class="card-header">spelartruppen</div>
					<div class="card-body">
						<template v-if="!loading">
							<table class="table table-sm mb-0">
								<thead class="thead-dark">
									<tr>
										<th scope="col">namn</th>
										<th scope="col">tröjnummer</th>
										<th scope="col">i antal startelvor</th>
										<th scope="col">antal positioner</th>
										<th scope="col">attribut</th>
										<th scope="col" v-if="loggedInUserIsAdmin"></th>
									</tr>
								</thead>
								<tbody>
									<tr v-for="player in players" v-bind:key="player.id">
										<td>{{ player.fullName }}</td>
										<td>{{ player.kitNumber }}</td>
										<td>{{ player.inNumberOfStartingElevens }}</td>
										<td>{{ player.availablePositions.length }}</td>
										<td>
											<span class="badge badge-primary" v-if="player.isCaptain">kapten</span>
											<span class="badge badge-danger" v-if="player.isHereOnLoan">inlånad</span>
											<span class="badge badge-warning" v-if="player.isOutOnLoan">utlånad</span>
											<span class="badge badge-danger" v-if="player.isSold">såld</span>
										</td>
										<td v-if="loggedInUserIsAdmin">
											<modal v-bind="{ id: `edit-${player.id}`, header: player.name }">
												<template slot="btn">
													<a data-toggle="modal" :data-target="'#' + `edit-${player.id}`">
														<span class="icon icon-edit"></span>
													</a>
												</template>
												<template slot="body">
													<form-component :player="player" :save="update" />
												</template>
											</modal>
										</td>
									</tr>
								</tbody>
							</table>
							<div class="row mt-3" v-if="loggedInUserIsAdmin">
								<div class="col">
									<modal v-bind="{ id: 'new-player', header: 'ny spelare' }">
										<template slot="btn">
											<button data-toggle="modal" :data-target="'#new-player'" class="btn btn-sm btn-primary btn-block">ny spelare</button>
										</template>
										<template slot="body">
											<form-component :save="create" />
										</template>
									</modal>
								</div>
							</div>
						</template>
						<template v-else>
							<loader v-bind="{ background: '#699ED0' }" />
						</template>
					</div>
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

import { ViewModel, Result, ResultType } from '../../../model/common'
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
import { Stretch as Loader } from 'vue-loading-spinner'

@Component({
	components: {
		Loader, Modal, FormComponent
	}
})
export default class Wrapper extends Vue {
	@State(state => state.vm) vm: ViewModel
	@ModuleGetter(GET_PLAYERS) players: Player[]
	@ModuleAction(FETCH_PLAYERS) loadPlayers: () => Promise<void>
	@ModuleAction(FETCH_POSITIONS) loadPositions: () => Promise<void>
	@ModuleAction(UPDATE_PLAYER) updatePlayer: (payload: { player: CrudPlayer }) => Promise<Result[]>
	@ModuleAction(CREATE_PLAYER) createPlayer: (payload: { player: CrudPlayer }) => Promise<Result[]>

	loading: boolean = true

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