<template>
	<div class="card">
		<div class="card-header">
			startelvor
		</div>
		<div class="card-body">
			<template v-if="!loading">
				<div class="row">
					<template v-if="lineups.length > 0 || noLineupsFound">
						<div class="col-sm-12" :class="{ 'col-lg-6 col-md-6': userIsLoggedInUser, 'col-lg-12 col-md-6': !userIsLoggedInUser }">
							<select class="form-control form-control-sm" v-model="selectedLineupId" @change="changeLineup" :disabled="inCreate || !lineups.length > 0">
								<option value="0">{{ lineups.length > 0 ? 'välj en startelva' : 'hittade inga startelvor' }}</option>
								<option v-for="lineup in lineups" :value="lineup.id">{{ lineup.formation.name }}, skapad {{ lineup.created }}</option>
							</select>
						</div>
					</template>
					<template v-else>
						<div class="col-sm-12" :class="{ 'col-lg-6 col-md-6': userIsLoggedInUser, 'col-lg-12 col-md-6': !userIsLoggedInUser }">
							<button class="btn btn-sm btn-primary btn-block" v-on:click="load">ladda startelvor</button>
						</div>
					</template>
					<template v-if="userIsLoggedInUser">
						<div class="col-sm-12" v-if="!inCreate" :class="{ 'col-lg-6 col-md-6': userIsLoggedInUser, 'col-lg-12 col-md-6': !userIsLoggedInUser }">
							<button class="btn btn-sm btn-success btn-block" v-on:click="setInCreate(true)">skapa ny startelva</button>
						</div>
						<div class="col-sm-12" v-if="inCreate" :class="{ 'col-lg-6 col-md-6': userIsLoggedInUser, 'col-lg-12 col-md-6': !userIsLoggedInUser }">
							<button class="btn btn-sm btn-danger btn-block" v-on:click="setInCreate(false)">avbryt</button>
						</div>
					</template>
				</div>
				<template v-if="selectedLineupId > 0">
					<div class="mt-3">
						<formation-component :formation="selectedLineup.formation" :players="selectedLineup.players" />
					</div>
				</template>
				<div class="row mt-3" v-if="inCreate">
					<div class="col">
						<select class="form-control form-control-sm" v-model="selectedFormationId" @change="changeFormation">
							<option value="0" selected>välj en formation</option>
							<option v-for="formation in formations" :value="formation.id">{{ formation.name }}</option>
						</select>
					</div>
				</div>
				<template v-if="inCreate && selectedFormationId > 0">
					<div class="row mt-3">
						<div class="col">
							<formation-component :formation="selectedFormation" />
							<results :items="results" class="pt-3" />
							<button class="btn btn-success btn-sm btn-block mt-3" :disabled="!canCreate" v-on:click="submit">skapa</button>
						</div>
					</div>
				</template>
			</template>
			<template v-else>
				<loader v-bind="{ background: '#699ED0' }" />
			</template>
		</div>
	</div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Mutation, Action, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('squad', Getter)
const ModuleAction = namespace('squad', Action)
const ModuleMutation = namespace('squad', Mutation)

import { Lineup, Player, Formation } from '../../model/squad/'
import { Lineup as CrudLineup } from '../../model/squad/crud'
import { ProfileViewModel, Result, ResultType } from '../../model/common'

import Results from '../../components/results.vue'

import {
	GET_PLAYERS,
	GET_POSITIONS,
	GET_FORMATIONS,
	GET_LINEUPS,
	GET_NEW_LINEUP,
	GET_SELECTED_LINEUP,
	FETCH_LINEUPS_ON_USER,
	FETCH_LINEUP,
	FETCH_FORMATIONS,
	FETCH_PLAYERS,
	FETCH_POSITIONS,
	CREATE_LINEUP,
	SET_NEW_LINEUP,
	SET_SELECTED_LINEUP
} from '../../modules/squad/types'

import FormationComponent from './formation.vue'

@Component({
	props: {
		preSelectedLineupId: {
			type: Number,
			default: 0
		}
	},
	components: {
		FormationComponent, Results
	}
})
export default class Lineups extends Vue {
	@State(state => state.vm) vm: ProfileViewModel

	@ModuleGetter(GET_LINEUPS) lineups: Lineup[]
	@ModuleGetter(GET_PLAYERS) players: Player[]
	@ModuleGetter(GET_FORMATIONS) formations: Formation[]
	@ModuleGetter(GET_NEW_LINEUP) newLineup: CrudLineup
	@ModuleGetter(GET_SELECTED_LINEUP) selectedLineup: Lineup

	@ModuleAction(FETCH_PLAYERS) loadPlayers: () => Promise<void>
	@ModuleAction(FETCH_FORMATIONS) loadFormations: () => Promise<void>
	@ModuleAction(FETCH_POSITIONS) loadPositions: () => Promise<void>
	@ModuleAction(FETCH_LINEUPS_ON_USER) loadLineups: (payload: { userId: string }) => Promise<void>
    @ModuleAction(FETCH_LINEUP) loadLineup: (payload: { id: number }) => Promise<Lineup>
	@ModuleAction(CREATE_LINEUP) create: (payload: { lineup: CrudLineup }) => Promise<Result[]>

	@ModuleMutation(SET_NEW_LINEUP) setNewLineup: (payload: { formationId: number }) => void
	@ModuleMutation(SET_SELECTED_LINEUP) setSelectedLineup: (lineup: Lineup) => void

	preSelectedLineupId: number

	selectedLineupId: number = 0
	selectedFormationId: number = 0

	loading: boolean = false
	inCreate: boolean = false
	noLineupsFound: boolean = false

	results: Result[] = []

	mounted() {
		if (this.preSelectedLineupId > 0) {
			new Promise<void>((resolve, reject) => {
				if (!this.lineups || this.lineups.length == 0)
					this.loadLineups({ userId: this.userId }).then(() => resolve())
				else
					resolve()
			}).then(() => {
				this.selectedLineupId = this.preSelectedLineupId
				this.changeLineup()
			})
		}
	}

	get userId() {
		return this.vm.selectedUserId ? this.vm.selectedUserId : this.vm.loggedInUser.id
	}
	get selectedFormation() {
		return this.formations.filter((f: Formation) => f.id == this.selectedFormationId)[0]
	}
	get canCreate() {
		return this.newLineup.players.length == 11
	}
	get userIsLoggedInUser() {
		return this.vm.selectedUserId ? this.vm.selectedUserId == this.vm.loggedInUser.id ? true : false : true
	}

	load() {
		this.loading = true
		this.loadLineups({ userId: this.userId })
			.then(() => {
				this.noLineupsFound = this.lineups.length == 0
				this.loading = false
			})
	}

	changeLineup() {
		if (this.selectedLineupId > 0) 
			this.setSelectedLineup(this.lineups.filter((l: Lineup) => l.id == this.selectedLineupId)[0])
		else
			this.setSelectedLineup(new Lineup())
	}

	changeFormation() {
		if (this.selectedFormationId > 0)
			this.setNewLineup({ formationId: this.selectedFormationId })
		else
			this.setNewLineup({ formationId: 0 })
	}

	setInCreate(state: boolean): Promise<void> {
		return new Promise<void>((resolve, reject) => {
			if (state && (this.formations.length == 0 || this.players.length == 0)) {
				this.loading = true
				Promise.all([this.loadPlayers(), this.loadFormations()])
					.then(() => {
						this.loading = false
						resolve()
					})
			} else {
				this.setNewLineup({ formationId: 0 })
				resolve()
			}
		}).then(() => {
			this.inCreate = state
			this.selectedLineupId = 0
			this.selectedFormationId = 0
		})
	}

	submit() {
		if (this.canCreate) {
			this.loading = true
			this.create({ lineup: { formationId: this.selectedFormationId, players: this.newLineup.players, createdByUserId: this.vm.loggedInUser.id } })
				.then((res: Result[]) => {
					this.loadLineups({ userId: this.userId })
						.then(() => {
							this.setInCreate(false)
								.then(() => {
									this.selectedLineupId = this.lineups[this.lineups.length - 1].id
									this.changeLineup()
									this.loading = false
									this.results = res
								})
						})
				})
		}
	}
}
</script>