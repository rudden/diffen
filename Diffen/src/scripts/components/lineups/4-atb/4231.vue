<template>
	<div>
		<!-- attack -->
		<div class="row">
			<div class="col-lg col-md"></div>
			<div class="col-sm col-lg-4 col-md-4 pl-0 pr-0"><player-card :player="canf.player" :position="canf.position" /></div>
			<div class="col-lg col-md"></div>
		</div>
		<!-- midfield -->
		<div class="row">
			<div class="col-sm col-lg-3 col-md-3"><player-card :player="vm.player" :position="vm.position" /></div>
			<div class="col-lg col-md"></div>
			<div class="col-sm col-lg-4 col-md-4 pl-0 pr-0"><player-card :player="com.player" :position="com.position" /></div>
			<div class="col-lg col-md"></div>
			<div class="col-sm col-lg-3 col-md-3"><player-card :player="hm.player" :position="hm.position" /></div>
		</div>
		<div class="row">
			<div class="col-lg col-md"></div>
			<div class="col-sm col-lg-3 col-md-3 pl-0"><player-card :player="vcdm.player" :position="vcdm.position" /></div>
			<div class="col-sm col-lg-3 col-md-3 pr-0"><player-card :player="hcdm.player" :position="hcdm.position" /></div>
			<div class="col-lg col-md"></div>
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