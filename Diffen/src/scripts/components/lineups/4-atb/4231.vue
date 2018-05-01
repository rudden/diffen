<template>
	<div>
		<!-- attack -->
		<div class="formation-section-wrapper">
			<player-card :player="canf.player" :position="canf.position" />
		</div>
		<!-- midfield -->
		<div class="formation-section-wrapper">
			<player-card :player="vm.player" :position="vm.position" class="mr-5" />
			<player-card :player="com.player" :position="com.position" />
			<player-card :player="hm.player" :position="hm.position" class="ml-5" />
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
export default class FourTwoThreeOne extends Vue {
	players: PlayerToLineup[]
	getPlayer: Function

	get vcdm() { return this.getPlayer('VCDM') }
	get hcdm() { return this.getPlayer('HCDM') }
	get vm() { return this.getPlayer('VM') }
	get com() { return this.getPlayer('COM') }
	get hm() { return this.getPlayer('HM') }
	get canf() { return this.getPlayer('CANF') }
}
</script>