<template>
	<div>
		<!-- attack -->
		<div class="formation-section-wrapper">
			<player-card :player="vanf.player" :position="vanf.position" />
			<player-card :player="hanf.player" :position="hanf.position" />
		</div>
		<div class="formation-section-wrapper">
			<player-card :player="com.player" :position="com.position" />
		</div>
		<!-- midfield -->
		<div class="formation-section-wrapper">
			<player-card :player="vcdm.player" :position="vcdm.position" />
			<player-card :player="hcdm.player" :position="hcdm.position" />
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
export default class FiveTwoOneTwo extends Vue {
	players: PlayerToLineup[]
	getPlayer: Function

	get vcdm() { return this.getPlayer('VCDM') }
	get hcdm() { return this.getPlayer('HCDM') }
	get com() { return this.getPlayer('COM') }
	get vanf() { return this.getPlayer('VANF') }
	get hanf() { return this.getPlayer('HANF') }
}
</script>