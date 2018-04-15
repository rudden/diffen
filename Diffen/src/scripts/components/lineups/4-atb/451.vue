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
			<div class="col-sm col-lg col-md"><player-card :player="vm.player" :position="vm.position" /></div>
			<div class="col-sm col-lg col-md"><player-card :player="vcm.player" :position="vcm.position" /></div>
			<div class="col-sm col-lg col-md pl-0 pr-0"><player-card :player="cm.player" :position="cm.position" /></div>
			<div class="col-sm col-lg col-md "><player-card :player="hcm.player" :position="hcm.position" /></div>
			<div class="col-sm col-lg col-md"><player-card :player="hm.player" :position="hm.position" /></div>
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
export default class FourFiveOne extends Vue {
	players: PlayerToLineup[]
	getPlayer: Function

	get cm() { return this.getPlayer('CM') }
	get hcm() { return this.getPlayer('HCM') }
	get vcm() { return this.getPlayer('VCM') }
	get vm() { return this.getPlayer('VM') }
	get hm() { return this.getPlayer('HM') }
	get canf() { return this.getPlayer('CANF') }
}
</script>