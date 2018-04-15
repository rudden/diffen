<template>
	<div>
		<div class="row">
			<div class="col-sm col-lg-3 col-md-3"><player-card :player="vyb.player" :position="vyb.position" /></div>
			<div class="col-lg col-md"></div>
			<div class="col-sm col-lg-3 col-md-3"><player-card :player="hyb.player" :position="hyb.position" /></div>
		</div>
		<div class="row">
			<div class="col-lg col-md"></div>
			<div class="col-sm col-lg-3 col-md-3"><player-card :player="vmb.player" :position="vmb.position" /></div>
			<div class="col-sm col-lg-4 col-md-4"><player-card :player="cmb.player" :position="cmb.position" /></div>
			<div class="col-sm col-lg-3 col-md-3"><player-card :player="hmb.player" :position="hmb.position" /></div>
			<div class="col-lg col-md"></div>
		</div>
		<div class="row">
			<div class="col-lg col-md"></div>
			<div class="col-sm col-lg-4 col-md-4"><player-card :player="mv.player" :position="mv.position" /></div>
			<div class="col-lg col-md"></div>
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
	get hyb() { return this.getPlayer('HYB') }
	get hmb() { return this.getPlayer('HMB') }
	get cmb() { return this.getPlayer('CMB') }
	get vmb() { return this.getPlayer('VMB') }
	get vyb() { return this.getPlayer('VYB') }
}
</script>