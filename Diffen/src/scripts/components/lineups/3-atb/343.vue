<template>
	<div>
		<!-- attack -->
		<div class="formation-section-wrapper">
			<player-card :player="canf.player" :position="canf.position" />
		</div>
		<div class="formation-section-wrapper-normal">
			<player-card :player="hy.player" :position="hy.position" class="float-left ml-5" style="margin-top: -1rem" />
			<player-card :player="vy.player" :position="vy.position" class="float-right mr-5" style="margin-top: -1rem" />
		</div>
		<!-- midfield -->
		<div class="formation-section-wrapper-normal">
			<player-card :player="vyb.player" :position="vyb.position" class="float-right" style="margin-top: -1rem" />
			<player-card :player="hyb.player" :position="hyb.position" class="float-left" style="margin-top: -1rem" />
		</div>
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
export default class ThreeFourThree extends Vue {
	players: PlayerToLineup[]
	getPlayer: Function

	get hcdm() { return this.getPlayer('HCDM') }
	get vcdm() { return this.getPlayer('VCDM') }
	get hyb() { return this.getPlayer('HYB') }
	get vyb() { return this.getPlayer('VYB') }
	get hy() { return this.getPlayer('HY') }
	get canf() { return this.getPlayer('CANF') }
	get vy() { return this.getPlayer('VY') }
}
</script>