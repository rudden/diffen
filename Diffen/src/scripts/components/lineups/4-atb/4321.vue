<template>
	<div>
		<!-- attack -->
		<div class="formation-section-wrapper">
			<player-card :player="canf.player" :position="canf.position" />
		</div>
		<!-- midfield -->
		<div class="formation-section-wrapper">
			<player-card :player="vm.player" :position="vm.position" class="mr-3" />
			<player-card :player="hm.player" :position="hm.position" class="ml-3" />
		</div>
		<div class="formation-section-wrapper">
			<player-card :player="vcm.player" :position="vcm.position" />
			<player-card :player="cm.player" :position="cm.position" />
			<player-card :player="hcm.player" :position="hcm.position" />
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
export default class FourThreeTwoOne extends Vue {
	players: PlayerToLineup[]
	getPlayer: Function

	get vcm() { return this.getPlayer('VCM') }
	get hcm() { return this.getPlayer('HCM') }
	get cm() { return this.getPlayer('CM') }
	get vm() { return this.getPlayer('VM') }
	get hm() { return this.getPlayer('HM') }
	get canf() { return this.getPlayer('CANF') }
}
</script>