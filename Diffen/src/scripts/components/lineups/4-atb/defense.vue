<template>
	<div>
		<div class="formation-section-wrapper">
			<player-card :player="vb.player" :position="vb.position"/>
			<player-card :player="vmb.player" :position="vmb.position" class="mr-1 ml-2" />
			<player-card :player="hmb.player" :position="hmb.position" class="ml-1 mr-2" />
			<player-card :player="hb.player" :position="hb.position" />
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
	get hb() { return this.getPlayer('HB') }
	get hmb() { return this.getPlayer('HMB') }
	get vmb() { return this.getPlayer('VMB') }
	get vb() { return this.getPlayer('VB') }
}
</script>