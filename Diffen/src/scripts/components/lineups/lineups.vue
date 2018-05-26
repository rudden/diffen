<template>
	<div>
		<template v-if="!loading">
			<div class="row">
				<template v-if="!inCreate">
					<div class="col-11 pr-0">
						<select class="form-control form-control-sm" v-model="selectedLineupId" @change="changeLineup" :disabled="!filteredLineups.length > 0">
							<option value="0">{{ !filteredLineups.length > 0 ? 'Hittade inga startelvor' : 'Välj en startelva' }}</option>
							<option v-for="lineup in filteredLineups" :value="lineup.id" :key="lineup.id">{{ lineup.formation.name }}, skapad {{ lineup.created }}</option>
						</select>
					</div>
				</template>
				<template v-if="inCreate">
					<div class="col-11 pr-0">
						<select class="form-control form-control-sm" v-model="selectedFormationId" @change="changeFormation">
							<option value="0" selected>Välj en formation</option>
							<option v-for="formation in formations" :value="formation.id" :key="formation.id">{{ formation.name }}</option>
						</select>
					</div>
				</template>
				<div class="pl-0 col-1" v-if="userIsLoggedInUser">
					<span class="icon float-right" :class="{ 'icon-plus': !inCreate, 'icon-minus': inCreate }" v-on:click="setInCreate(!inCreate)" style="cursor: pointer" v-tooltip="`${!inCreate ? 'Skapa ny startelva' : 'Välj befintlig startelva'}`"></span>
				</div>
			</div>
			<template v-if="selectedLineupId > 0">
				<div class="mt-3">
					<formation-component :formation="selectedLineup.formation" :players="selectedLineup.players" />
					<results :items="results" class="pt-3" />
				</div>
			</template>
			<template v-if="inCreate && selectedFormationId > 0">
				<div class="row mt-3">
					<div class="col">
						<formation-component :formation="selectedFormation" />
						<button class="btn btn-success btn-sm btn-block mt-3" :disabled="!canCreate" v-on:click="submit" v-if="showCreateButton">{{ buttonText }}</button>
					</div>
				</div>
			</template>
		</template>
		<template v-else>
			<loader v-bind="{ background: '#699ED0' }" />
		</template>
	</div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Mutation, Action, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('squad', Getter)
const ModuleAction = namespace('squad', Action)
const ModuleMutation = namespace('squad', Mutation)

import { Lineup, Player, Formation, LineupType } from '../../model/squad/'
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
		lineupType: {
			type: String,
			default: 'Fiction'
		},
		buttonText: {
			type: String,
			default: 'Skapa'
		},
		buttonAction: Function,
		preSelectedLineupId: {
			type: Number,
			default: 0
		},
		preDefinedLineup: Object,
		showCreateButton: {
			type: Boolean,
			default: true
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

	lineupType: string
	buttonText: string
	buttonAction: () => void
	preSelectedLineupId: number
	preDefinedLineup: Lineup
	showCreateButton: boolean

	selectedLineupId: number = 0
	selectedFormationId: number = 0

	loading: boolean = false
	inCreate: boolean = false
	noLineupsFound: boolean = false
	typeOfLineup: LineupType = LineupType.Real
	preSelectedLineup?: Lineup

	results: Result[] = []

	mounted() {
		this.typeOfLineup = LineupType[this.lineupType as keyof typeof LineupType]
		if (this.preSelectedLineupId > 0) {
			new Promise<void>((resolve, reject) => {
				if (this.typeOfLineup == this.fictionLineupType) {
					if (!this.lineups || this.lineups.length == 0)
						this.loadLineups({ userId: this.userId }).then(() => resolve())
					else
						resolve()
				}
			}).then(() => {
				this.selectedLineupId = this.preSelectedLineupId
				this.changeLineup()
			})
		}
		if (this.preDefinedLineup && this.preDefinedLineup.id) {
			this.preSelectedLineup = this.preDefinedLineup
			this.selectedLineupId = this.preDefinedLineup.id
			this.changeLineup()
			return
		}
		if (this.typeOfLineup == this.realLineupType) {
			this.setInCreate(true)
		}
		if (!this.lineups || this.lineups.length == 0) {
			this.load()
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
	get realLineupType() {
		return LineupType.Real
	}
	get fictionLineupType() {
		return LineupType.Fiction
	}
	get filteredLineups() {
		return this.lineups.filter((l: Lineup) => l.type == this.typeOfLineup)
	}

	load() {
		this.loading = true
		this.loadLineups({ userId: this.userId })
			.then(() => {
				this.noLineupsFound = this.filteredLineups.length == 0
				this.loading = false
			})
	}

	changeLineup() {
		if (this.selectedLineupId > 0) {
			if (this.preSelectedLineup) {
				this.setSelectedLineup(this.preSelectedLineup)
			} else {
				this.setSelectedLineup(this.lineups.filter((l: Lineup) => l.id == this.selectedLineupId)[0])
			}
		}
		else
			this.setSelectedLineup(new Lineup())
	}

	changeFormation() {
		if (this.selectedFormationId > 0) {
			this.setNewLineup({ formationId: this.selectedFormationId })
		}		
		else {
			this.setNewLineup({ formationId: 0 })
		}
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
			if (state)
				this.setSelectedLineup(new Lineup())
		})
	}

	submit() {
		if (this.buttonAction) {
			this.buttonAction()
		} else {
			if (this.canCreate) {
				this.loading = true
				this.create({ lineup: { formationId: this.selectedFormationId, players: this.newLineup.players, createdByUserId: this.vm.loggedInUser.id, type: LineupType.Fiction } })
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
}
</script>