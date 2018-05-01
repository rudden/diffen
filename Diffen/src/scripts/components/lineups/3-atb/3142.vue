<template>
	<div>
		<!-- attack -->
		<div class="formation-section-wrapper">
			<player-card :player="vanf.player" :position="vanf.position" />
			<player-card :player="hanf.player" :position="hanf.position" />
		</div>
		<!-- midfield -->
		<div class="formation-section-wrapper">
			<player-card :player="vyb.player" :position="vyb.position" class="mr-5" />
			<player-card :player="vcm.player" :position="vcm.position" />
			<player-card :player="hcm.player" :position="hcm.position" />
			<player-card :player="hyb.player" :position="hyb.position" class="ml-5" />
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
export default class ThreeOneFourTwo extends Vue {
	players: PlayerToLineup[]
	getPlayer: Function

	get cdm() { return this.getPlayer('CDM') }
	get vcm() { return this.getPlayer('VCM') }
	get hyb() { return this.getPlayer('HYB') }
	get vyb() { return this.getPlayer('VYB') }
	get hcm() { return this.getPlayer('HCM') }
	get vanf() { return this.getPlayer('VANF') }
	get hanf() { return this.getPlayer('HANF') }
}
</script>