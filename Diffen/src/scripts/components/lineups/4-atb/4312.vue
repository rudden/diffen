<template>
	<div>
		<!-- attack -->
		<div class="formation-section-wrapper">
			<player-card :player="vanf.player" :position="vanf.position" />
			<player-card :player="hanf.player" :position="hanf.position" />
		</div>
		<!-- midfield -->
		<div class="formation-section-wrapper">
			<player-card :player="com.player" :position="com.position" />
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
export default class FourThreeOneTwo extends Vue {
	players: PlayerToLineup[]
	getPlayer: Function

	get vcm() { return this.getPlayer('VCM') }
	get hcm() { return this.getPlayer('HCM') }
	get cm() { return this.getPlayer('CM') }
	get com() { return this.getPlayer('COM') }
	get vanf() { return this.getPlayer('VANF') }
	get hanf() { return this.getPlayer('HANF') }
}
</script>