<template>
	<div class="m-1">
		<template v-if="player.name">
			<div class="card mb-3">
				<div class="card-body">
					<h6 class="card-title">
						{{ player.name }}
						<span class="ml-2" v-if="hasAttribute">
							<span class="badge badge-primary" v-if="player.isCaptain">kapten</span>
							<span class="badge badge-danger" v-else-if="player.isHereOnLoan">inlånad</span>
							<span class="badge badge-warning" v-else-if="player.isOutOnLoan">utlånad</span>
							<span class="badge badge-danger" v-else-if="player.isSold">såld</span>
						</span>
					</h6>
				</div>
			</div>
		</template>
		<template v-else>
			<template v-if="kvpPlayers.length > 0">
				<template v-if="!selected">
					<div class="card" style="width: 100%">
						<div class="card-body p-0">
							<input :id="position.name" class="form-control mr-sm-2" type="text" :placeholder="position.name">
							<typeahead v-model="selected" :target="'#' + position.name" :data="kvpPlayers" item-key="value" force-select />
						</div>
					</div>
				</template>
				<template v-else>
					<div class="card mb-3">
						<div class="card-body pt-1 pb-1">
							<strong>{{ selectedPlayerName }}</strong>
							<a v-on:click="deSelect" class="float-right ml-2" style="cursor: pointer">
								<span class="icon icon-trash"></span>
							</a>
						</div>
					</div>
				</template>
			</template>
		</template>
	</div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component, Watch } from 'vue-property-decorator'
import { Getter, Action, Mutation, namespace } from 'vuex-class'

const ModuleGetter = namespace('squad', Getter)
const ModuleAction = namespace('squad', Action)
const ModuleMutation = namespace('squad', Mutation)

import { GET_PLAYERS, GET_NEW_LINEUP, SET_PLAYER_TO_LINEUP } from '../../modules/squad/types'

import { Player, Position } from '../../model/squad'
import { KeyValuePair } from '../../model/common'

import { Typeahead } from 'uiv'
import { Lineup as CrudLineup, PlayerToLineup } from '../../model/squad/crud'

@Component({
	props: {
        player: Object,
        position: Object
    },
	components: {
        Typeahead
    }
})
export default class PlayerCard extends Vue {
	@ModuleGetter(GET_PLAYERS) players: Player[]
	@ModuleGetter(GET_NEW_LINEUP) newLineup: CrudLineup
	@ModuleMutation(SET_PLAYER_TO_LINEUP) setPlayerToLineup: (payload: { playerId: number, positionId: number }) => void
    
	player: Player
	position: Position

	selected: any = ''

	get hasAttribute() {
		return this.player.isCaptain || this.player.isHereOnLoan || this.player.isOutOnLoan || this.player.isSold
	}
	get availablePlayers() {
		let players = this.players.filter((player: Player) => player.availablePositions.map((position: Position) => position.id).includes(this.position.id))
		if (!this.newLineup.players)
			return players
		return players.filter((player: Player) => !this.newLineup.players.map((ptl: PlayerToLineup) => ptl.playerId).includes(player.id))
	}
	get kvpPlayers() {
		return this.availablePlayers.map((player: Player) => { 
			return { 
				key: player.id,
				value: player.fullName 
			}
		})
	}
	get selectedPlayerName(): string {
		if (this.selected) {
			return this.players.filter((player: Player) => player.id == this.selected.key)[0].name
		}
		return ''
	}

	@Watch('selected')
		onChange() {
			if (this.selected && this.selected.key) {
				this.setPlayerToLineup({ playerId: parseInt(this.selected.key), positionId: this.position.id })
			}
		}

	deSelect() {
		this.setPlayerToLineup({ playerId: 0, positionId: this.position.id })
		this.selected = ''
	}
}
</script>

<style lang="scss" scoped>
.card-body {
	padding: 10px;
	.card-title {
		margin-bottom: 0;
	}
}
</style>
