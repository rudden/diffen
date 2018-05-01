<template>
	<div>
		<!-- attack -->
		<div class="formation-section-wrapper">
			<player-card :player="canf.player" :position="canf.position" />
		</div>
		<!-- midfield -->
		<div class="formation-section-wrapper">
			<player-card :player="com.player" :position="com.position" />
		</div>
		<div class="formation-section-wrapper">
			<player-card :player="vm.player" :position="vm.position" />
			<player-card :player="vcm.player" :position="vcm.position" />
			<player-card :player="hcm.player" :position="hcm.position" />
			<player-card :player="hm.player" :position="hm.position" />
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
export default class FourFourOneOne extends Vue {
	players: PlayerToLineup[]
	getPlayer: Function

	get vcm() { return this.getPlayer('VCM') }
	get hcm() { return this.getPlayer('HCM') }
	get vm() { return this.getPlayer('VM') }
	get hm() { return this.getPlayer('HM') }
	get com() { return this.getPlayer('COM') }
	get canf() { return this.getPlayer('CANF') }
}
</script>