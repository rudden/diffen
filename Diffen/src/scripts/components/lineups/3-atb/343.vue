<template>
	<div>
		<!-- attack -->
		<div class="row">
			<div class="col-lg col-md"></div>
			<div class="col-sm col-lg-4 col-md-4 pl-0 pr-0"><player-card :player="canf.player" :position="canf.position" /></div>
			<div class="col-lg col-md"></div>
		</div>
		<div class="row">
			<div class="col-lg col-md"></div>
			<div class="col-sm col-lg-3 col-md-3 pl-0 pr-0"><player-card :player="vy.player" :position="vy.position" /></div>
			<div class="col-lg col-md"></div>
			<div class="col-sm col-lg-3 col-md-3"></div>
			<div class="col-lg col-md"></div>
			<div class="col-sm col-lg-3 col-md-3 pl-0 pr-0"><player-card :player="hy.player" :position="hy.position" /></div>
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