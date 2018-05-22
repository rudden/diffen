<template>
    <div>
        <div class="form-group">
            <div class="row">
                <legend class="col-sm-2 col-form-label pt-0"><strong>Spelare</strong></legend>
                <div class="col-sm-10">
                    <select class="form-control form-control-sm" v-model="selectedPlayerId">
                        <option value="0">Välj en spelare</option>
                        <option v-for="player in players" :value="player.id">{{ player.fullName }}</option>
                    </select>
                </div>
            </div>
        </div>
        <fieldset class="form-group">
            <div class="row">
                <legend class="col-sm-2 col-form-label pt-0"><strong>Typ</strong></legend>
                <div class="col-sm-10">
                    <div class="form-check form-check-inline">
                        <input class="form-check-input" type="radio" :id="`goal-${guid}`" v-model="eventType" value="Goal" />
                        <label class="form-check-label" :for="`goal-${guid}`">Mål</label>
                    </div>
                    <div class="form-check form-check-inline">
                        <input class="form-check-input" type="radio" :id="`assist-${guid}`" v-model="eventType" value="Assist" />
                        <label class="form-check-label" :for="`assist-${guid}`">Assist</label>
                    </div>
                    <div class="form-check form-check-inline">
                        <input class="form-check-input" type="radio" :id="`yc-${guid}`" v-model="eventType" value="YellowCard" />
                        <label class="form-check-label" :for="`yc-${guid}`">Gult kort</label>
                    </div>
                    <div class="form-check form-check-inline">
                        <input class="form-check-input" type="radio" :id="`rc-${guid}`" v-model="eventType" value="RedCard" />
                        <label class="form-check-label" :for="`rc-${guid}`">Rött kort</label>
                    </div>
                    <div class="form-check form-check-inline">
                        <input class="form-check-input" type="radio" :id="`s_in-${guid}`" v-model="eventType" value="SubstituteIn" />
                        <label class="form-check-label" :for="`s_in-${guid}`">Byte in</label>
                    </div>
                    <div class="form-check form-check-inline">
                        <input class="form-check-input" type="radio" :id="`s_out-${guid}`" v-model="eventType" value="SubstituteOut" />
                        <label class="form-check-label" :for="`s_out-${guid}`">Byte ut</label>
                    </div>
                </div>
            </div>
        </fieldset>
        <div class="form-group mb-0">
            <div class="row">
                <legend class="col-sm-2 col-form-label pt-0"><strong>I minut</strong></legend>
                <div class="col-sm-10">
                    <input class="form-control form-control-sm" type="number" minlength="1" max="120" v-model="inMinute" placeholder="Hände i matchminut">
                </div>                
            </div>
        </div>
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component, Watch } from 'vue-property-decorator'
import { Getter, Action, Mutation, State, namespace } from 'vuex-class'

import { PageViewModel } from '../model/common'
import { Game, GameType, PlayerEvent, GameEventType, Player } from '../model/squad'
import { Game as CrudGame, PlayerEvent as CrudPlayerEvent } from '../model/squad/crud'

const ModuleGetter = namespace('squad', Getter)
const ModuleAction = namespace('squad', Action)
const ModuleMutation = namespace('squad', Mutation)

import { GET_PLAYERS, SET_GAME_EVENT, CHANGE_GAME_EVENT } from '../modules/squad/types'

@Component({
    props: {
        event: Object
    }
})
export default class PlayerEventsForm extends Vue {
    @State(state => state.vm) vm: PageViewModel
    @ModuleGetter(GET_PLAYERS) players: Player[]
    @ModuleMutation(SET_GAME_EVENT) setGameEvent: (event: CrudPlayerEvent) => void
    @ModuleMutation(CHANGE_GAME_EVENT) changeGameEvent: (event: CrudPlayerEvent) => void

    event: CrudPlayerEvent

    selectedPlayerId: number = 0

    eventType: string = 'Goal'
    inMinute: number = 1

    guid: string = (this as any).$helpers.guid()

    mounted() {
        if (this.event.playerId > 0) {
            this.inMinute = this.event.inMinute
            this.selectedPlayerId = this.event.playerId
            this.eventType = GameEventType[this.event.type]
        }
    }

    @Watch('selectedPlayerId')
        changePlayer() {
            this.update()
        }
    
    @Watch('eventType')
        changeType() {
            this.update()
        }
    
    @Watch('inMinute')
        changeMinute() {
            this.update()
        }

    update() {
        this.event.playerId = <number>this.selectedPlayerId
        this.event.type = GameEventType[this.eventType as keyof typeof GameEventType]
        this.event.inMinute = this.inMinute
        this.changeGameEvent(this.event)
    }
}
</script>
