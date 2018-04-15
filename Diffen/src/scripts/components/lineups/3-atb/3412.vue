<template>
	<div>
		<!-- attack -->
		<div class="row">
			<div class="col-lg col-md"></div>
			<div class="col-sm col-lg-4 col-md-4 pl-0"><player-card :player="vanf.player" :position="vanf.position" /></div>
			<div class="col-sm col-lg-4 col-md-4 pr-0"><player-card :player="hanf.player" :position="hanf.position" /></div>
			<div class="col-lg col-md"></div>
		</div>
        <div class="row">
			<div class="col-lg col-md"></div>
			<div class="col-sm col-lg-4 col-md-4 pl-0 pr-0"><player-card :player="com.player" :position="com.position" /></div>
			<div class="col-lg col-md"></div>
		</div>
		<!-- midfield -->
		<div class="row">
			<div class="col-sm col-lg-3 col-md-3 pr-0"><player-card :player="vyb.player" :position="vyb.position" /></div>
			<div class="col-lg col-md"></div>
			<div class="col-sm col-lg-3 col-md-3 pl-0"><player-card :player="hyb.player" :position="hyb.position" /></div>
		</div>
		<div class="row">
			<div class="col-lg col-md"></div>
			<div class="col-sm col-lg-4 col-md-4 pl-0 pr-0"><player-card :player="vcdm.player" :position="vcdm.position" /></div>
			<div class="col-lg col-md"></div>
			<div class="col-sm col-lg-4 col-md-4 pl-0 pr-0"><player-card :player="hcdm.player" :position="hcdm.position" /></div>
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
export default class ThreeFourOneTwo extends Vue {
	players: PlayerToLineup[]
	getPlayer: Function

	get vcdm() { return this.getPlayer('VCDM') }
	get hcdm() { return this.getPlayer('HCDM') }
	get hyb() { return this.getPlayer('HYB') }
	get vyb() { return this.getPlayer('VYB') }
	get com() { return this.getPlayer('COM') }
	get vanf() { return this.getPlayer('VANF') }
	get hanf() { return this.getPlayer('HANF') }
}
</script>