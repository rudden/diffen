<template>
    <div class="card mb-4">
        <div class="card-body">
            <template v-if="loggedInUserIsAdminOrScissor">
                <div class="row mb-3">
                    <div class="col">
                        <modal v-bind="modalAttributes.newEvent">
                            <template slot="body">
                                <div class="row">
                                    <div class="col">
                                        <fieldset class="form-group">
                                            <div class="row">
                                                <legend class="col-sm-3 col-form-label pt-0">Typ av match</legend>
                                                <div class="col-sm-9">
                                                    <div class="form-check">
                                                        <input class="form-check-input" type="radio" id="cup" v-model="gameType" value="Cup" />
                                                        <label class="form-check-label" for="cup">Cup</label>
                                                    </div>
                                                    <div class="form-check">
                                                        <input class="form-check-input" type="radio" id="league" v-model="gameType" value="League" />
                                                        <label class="form-check-label" for="league">Allsvenskan</label>
                                                    </div>
                                                    <div class="form-check">
                                                        <input class="form-check-input" type="radio" id="training" v-model="gameType" value="Training" />
                                                        <label class="form-check-label" for="training">Träningsmatch</label>
                                                    </div>
                                                    <div class="form-check">
                                                        <input class="form-check-input" type="radio" id="europaLeague" v-model="gameType" value="EuropaLeague" />
                                                        <label class="form-check-label" for="europaLeague">Europa league</label>
                                                    </div>
                                                </div>
                                            </div>
                                        </fieldset>
                                        <div class="row">
                                            <div class="col">
                    							<date-picker v-model="crudGame.playedDate" :config="dpConfig" placeholder="när spelades matchen" :class="{ 'form-control-sm': true }" />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                <!-- <button class="btn btn-success btn-block btn-sm" :disabled="!canSave" v-on:click="onSave">Spara</button> -->
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </template>
                        </modal>
                    </div>
                </div>
                <hr />
            </template>
            <ul class="list-unstyled list-spaced mb-0" v-if="!loading">
                <li><strong>Skytteliga</strong></li>
                <li class="ellipsis" v-for="item in goals" :key="`goals-${item.player.id}`">
                    <span class="badge badge-secondary float-right">{{ item.events.length }}</span>
                    {{ item.player.fullName }}
                </li>
                <hr />
                <li><strong>Assistliga</strong></li>
                <li class="ellipsis" v-for="item in assists" :key="`assists-${item.player.id}`">
                    <span class="badge badge-secondary float-right">{{ item.events.length }}</span>
                    {{ item.player.fullName }}
                </li>
                <hr />
                <li><strong>Gula kort</strong></li>
                <li class="ellipsis" v-for="item in yellowCards" :key="`yellowCards-${item.player.id}`">
                    <span class="badge badge-secondary float-right">{{ item.events.length }}</span>
                    {{ item.player.fullName }}
                </li>
                <hr />
                <li><strong>Röda kort</strong></li>
                <li class="ellipsis" v-for="item in redCards" :key="`redCards-${item.player.id}`">
                    <span class="badge badge-secondary float-right">{{ item.events.length }}</span>
                    {{ item.player.fullName }}
                </li>
            </ul>
            <template v-else>
                <loader v-bind="{ background: '#699ED0' }" />
            </template>
        </div>
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Action, State, namespace } from 'vuex-class'

import { PageViewModel } from '../model/common'
import { Game, GameType, PlayerEvent, GameEventType, Player } from '../model/squad'
import { Game as CrudGame } from '../model/squad/crud'

const ModuleGetter = namespace('squad', Getter)
const ModuleAction = namespace('squad', Action)

import { GET_GAMES, GET_PLAYERS, FETCH_GAMES } from '../modules/squad/types'

interface PlayerItem {
    id: number
    fullName: string
}

interface Event {
    player: PlayerItem
    gameType: GameType
    eventType: GameEventType
}

interface ListItem {
    player: PlayerItem
    events: Event[]
}

import Modal from './modal.vue'

import DatePicker from 'vue-bootstrap-datetimepicker'

@Component({
    components: {
        Modal, DatePicker
    }
})
export default class PlayerEvents extends Vue {
    @State(state => state.vm) vm: PageViewModel
    @ModuleGetter(GET_GAMES) games: Game[]
    @ModuleGetter(GET_PLAYERS) players: Player[]
    @ModuleAction(FETCH_GAMES) loadGames: () => Promise<void>

    loading: boolean = true
    events: Event[] = []

    crudGame: CrudGame = new CrudGame()

    gameType: string = ''

    selectedGameType = GameType[this.gameType as keyof typeof GameType]

    modalAttributes: any = {
		newEvent: {
			attributes: {
				name: 'new-event',
				scrollable: true
			},
			header: 'Ny händelse',
			button: {
				classes: 'btn btn-sm btn-success btn-block float-right',
				text: 'Skapa ny spelarhändelse'
			}
		}
    }
    
    dpConfig: any = { 
		format: 'YYYY-MM-DD', 
		useCurrent: false, 
		locale: 'sv', 
		icons: { 
			next: 'icon icon-arrow-right',
			previous: 'icon icon-arrow-left' 
        },
        widgetPositioning: {
            vertical: 'top',
            horizontal: 'left'
        }
    }

    mounted() {
        this.loadGames().then(() => {
            this.loading = false
            this.games.forEach((game: Game) => {
                game.playerEvents.forEach((playerEvent: PlayerEvent) => {
                    this.events.push({
                        player: {
                            id: playerEvent.player.id,
                            fullName: playerEvent.player.fullName,
                        },
                        gameType: game.type,
                        eventType: playerEvent.eventType
                    })
                })
            })
        })
    }

    get loggedInUserIsAdminOrScissor(): boolean {
        return this.vm.loggedInUser.inRoles.some(role => role == 'Admin' || role == 'Scissor')
    }

    get cupType() { return GameType.Cup }
    get leagueCup() { return GameType.League }

    get goals() {
        return this.listify(GameEventType.Goal)
    }
    get assists() {
        return this.listify(GameEventType.Assist)
    }
    get yellowCards() {
        return this.listify(GameEventType.YellowCard)
    }
    get redCards() {
        return this.listify(GameEventType.RedCard)
    }

    listify(type: GameEventType) {
        let items: ListItem[] = []
        let events = this.events.filter((e: Event) => e.eventType == type)
        events.forEach((e: Event) => {
            if (items.map((t: ListItem) => t.player.id).includes(e.player.id))
                return
            items.push({
                player: e.player,
                events: events.filter((e2: Event) => e2.player.id == e.player.id)
            })
        })
        items.sort((a: ListItem, b: ListItem) => {
            return b.events.length - a.events.length
        })
        return items
    }
}
</script>
