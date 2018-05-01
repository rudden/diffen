<template>
	<div>
		<!-- attack -->
		<div class="formation-section-wrapper">
			<player-card :player="vanf.player" :position="vanf.position" class="mr-3" />
			<player-card :player="hanf.player" :position="hanf.position" class="ml-3" />
		</div>
		<div class="formation-section-wrapper">
			<player-card :player="com.player" :position="com.position" />
		</div>
		<!-- midfield -->
		<div class="formation-section-wrapper">
			<player-card :player="vcm.player" :position="vcm.position" class="mr-5" />
			<player-card :player="hcm.player" :position="hcm.position" class="ml-5" />
		</div>
		<div class="formation-section-wrapper">
			<player-card :player="cdm.player" :position="cdm.position" />
		</div>
		<!-- defense -->
		<defense :players="players" :get-player="getPlayer" />
	</div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'

import { PlayerToLineup } from '../../../model/squad'

import Defense from './defense.vue'
import PlayerCard from '../player.vue'

@Component({
	props: {
		players: Array,
		getPlayer: Function
	},
	components: {
		PlayerCard, Defense
	}
})
export default class FourOneTwoOneTwo extends Vue {
	players: PlayerToLineup[]
	getPlayer: Function

	get cdm() { return this.getPlayer('CDM') }
	get vcm() { return this.getPlayer('VCM') }
	get hcm() { return this.getPlayer('HCM') }
	get com() { return this.getPlayer('COM') }
	get vanf() { return this.getPlayer('VANF') }
	get hanf() { return this.getPlayer('HANF') }
}
</script>