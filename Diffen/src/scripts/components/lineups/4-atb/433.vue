<template>
	<div>
		<!-- attack -->
		<div class="formation-section-wrapper">
			<player-card :player="canf.player" :position="canf.position" />
		</div>
		<div class="formation-section-wrapper-normal">
			<player-card :player="vy.player" :position="vy.position" class="float-left" />
			<player-card :player="hy.player" :position="hy.position" class="float-right" />
		</div>
		<!-- midfield -->
		<div class="formation-section-wrapper">
			<player-card :player="cm.player" :position="cm.position" />
		</div>
		<div class="formation-section-wrapper">
			<player-card :player="vcm.player" :position="vcm.position" class="mr-3" />
			<player-card :player="hcm.player" :position="hcm.position" class="ml-3" />
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
export default class FourThreeThree extends Vue {
	players: PlayerToLineup[]
	getPlayer: Function

	get cm() { return this.getPlayer('CM') }
	get hcm() { return this.getPlayer('HCM') }
	get vcm() { return this.getPlayer('VCM') }
	get vy() { return this.getPlayer('VY') }
	get hy() { return this.getPlayer('HY') }
	get canf() { return this.getPlayer('CANF') }
}
</script>