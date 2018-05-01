<template>
	<div>
		<div class="formation-section-wrapper">
			<player-card :player="vmb.player" :position="vmb.position" />
			<player-card :player="cmb.player" :position="cmb.position" />
			<player-card :player="hmb.player" :position="hmb.position" />
		</div>
		<div class="formation-section-wrapper">
			<player-card :player="mv.player" :position="mv.position" />
		</div>
	</div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'

import { Player, PlayerToLineup, Position } from '../../../model/squad'

import PlayerCard from '../player.vue'

@Component({
	props: {
		players: Array,
		getPlayer: Function
	},
	components: {
		PlayerCard
	}
})
export default class Defense extends Vue {
	players: PlayerToLineup[]
	getPlayer: Function

	get mv() { return this.getPlayer('MV') }
	get hmb() { return this.getPlayer('HMB') }
	get cmb() { return this.getPlayer('CMB') }
	get vmb() { return this.getPlayer('VMB') }
}
</script>