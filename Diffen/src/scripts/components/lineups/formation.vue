<template>
	<div>
		<template v-if="!loading">
			<div class="card text-center">
				<div class="card-header">
					{{ formation.name }}
				</div>
				<div class="card-body pb-0 formation">
					<component v-bind:is="formation.componentName" :get-player="getPlayer" />
				</div> 
			</div>
		</template>
		<template v-else>
			<div class="mt-3 mb-0">
				<loader v-bind="{ background: '#699ED0' }" />
			</div>
		</template>
	</div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Action, namespace } from 'vuex-class'

const ModuleGetter = namespace('squad', Getter)
const ModuleAction = namespace('squad', Action)

import { GET_POSITIONS, GET_PLAYERS, FETCH_PLAYERS, FETCH_POSITIONS } from '../../modules/squad/types'

import { PlayerToLineup, Formation, Position, Player } from '../../model/squad'

/* lineups */
import ThreeFourThree from './3-atb/343.vue'
import ThreeFiveTwo from './3-atb/352.vue'
import ThreeOneFourTwo from './3-atb/3142.vue'
import ThreeFourOneTwo from './3-atb/3412.vue'

import FourFourTwo from './4-atb/442.vue'
import FourFiveOne from './4-atb/451.vue'
import FourFourOneOne from './4-atb/4411.vue'
import FourThreeThree from './4-atb/433.vue'
import FourThreeTwoOne from './4-atb/4321.vue'
import FourThreeOneTwo from './4-atb/4312.vue'
import FourTwoThreeOne from './4-atb/4231.vue'
import FourOneFourOne from './4-atb/4141.vue'
import FourOneTwoOneTwo from './4-atb/41212.vue'

import FiveThreeTwo from './5-atb/532.vue'
import FiveFourOne from './5-atb/541.vue'
import FiveTwoOneTwo from './5-atb/5212.vue'

@Component({
	props: {
		players: Array,
		formation: Object
	},
	components: {
		ThreeFourThree, ThreeOneFourTwo, ThreeFiveTwo, ThreeFourOneTwo,
		FourFourTwo, FourFiveOne, FourFourOneOne, FourThreeThree, FourThreeTwoOne, FourThreeOneTwo, FourTwoThreeOne, FourOneFourOne, FourOneTwoOneTwo,
		FiveThreeTwo, FiveFourOne, FiveTwoOneTwo
	}
})
export default class FormationComponent extends Vue {
	@ModuleGetter(GET_POSITIONS) positions: Position[]
	@ModuleGetter(GET_PLAYERS) statePlayers: Player[]
	@ModuleAction(FETCH_PLAYERS) loadPlayers: () => Promise<void>
	@ModuleAction(FETCH_POSITIONS) loadPositions: () => Promise<void>

	players: PlayerToLineup[]
	formation: Formation

	loading: boolean = false

	created() {
		if (this.positions.length == 0 || (this.statePlayers.length == 0 && !this.players)) {
			this.loading = true
			Promise.all([this.loadPlayers(), this.loadPositions()])
				.then(() => this.loading = false)
		}
	}

	getPlayer(position: string) {
		let pos = this.getPosition(position)
		if (!this.players) {
			return { 
				player: {},
				position: pos
			}
		}
		let player = this.players.filter((p: PlayerToLineup) => p.position.id == pos.id)[0].player
		return { 
			player: player,
			position: pos
		}
	}
	
	getPosition(position: string): Position {
		return this.positions.filter((p: Position) => p.name == position)[0]
	}
}
</script>