<template>
	<div>
		<!-- attack -->
		<div class="formation-section-wrapper">
			<player-card :player="vanf.player" :position="vanf.position" class="mr-2" />
			<player-card :player="hanf.player" :position="hanf.position" />
		</div>
		<!-- midfield -->
		<div class="formation-section-wrapper">
			<player-card :player="vm.player" :position="vm.position" />
			<player-card :player="vcm.player" :position="vcm.position" class="mr-1 ml-2" />
			<player-card :player="hcm.player" :position="hcm.position" class="ml-1 mr-2" />
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
export default class FourTwoTwo extends Vue {
	players: PlayerToLineup[]
	getPlayer: Function

	get hm() { return this.getPlayer('HM') }
	get hcm() { return this.getPlayer('HCM') }
	get vcm() { return this.getPlayer('VCM') }
	get vm() { return this.getPlayer('VM') }
	get vanf() { return this.getPlayer('VANF') }
	get hanf() { return this.getPlayer('HANF') }
}
</script>