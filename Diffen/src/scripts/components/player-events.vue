<template>
    <div class="card mb-4">
        <div class="card-body">
            <template v-if="loggedInUserIsAdminOrScissor">
                <div class="row mb-3">
                    <div class="col">
                        <div class="form-group">
                            <select class="form-control form-control-sm" v-model="selectedGameId">
                                <option value="0">Hantera händelser i en match</option>
                                <option v-for="game in games" :value="game.id">{{ getGameType(game.type) }} - {{ game.playedOn }}</option>
                            </select>
                        </div>
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
                                                        <label class="form-check-label" for="europaLeague">Europa League</label>
                                                    </div>
                                                </div>
                                            </div>
                                        </fieldset>
                                        <div class="form-group">
                                            <date-picker v-model="selectedDate" :config="dpConfig" placeholder="när spelades matchen" :class="{ 'form-control-sm': true }" />
                                        </div>
                                        <div class="row col">
                                            <button class="btn btn-sm btn-success" v-on:click="newEvent">Lägg till matchhändelse</button>
                                        </div>
                                        <div class="form-group mt-3 mb-0" v-for="(item, index) in eventComponents" :key="index" v-if="eventComponents.length > 0">
                                            <div class="row">
                                                <legend class="col-sm-1 col-form-label pt-0">{{ index + 1 }}.</legend>
                                                <div class="col-sm-10">
                                                    <component :is="item.component" v-bind="{ event: item.event }" />
                                                </div>
                                                <div class="col-sm-1">
                                                    <span v-on:click="removeEvent(index, item.event)" class="icon icon-trash" style="cursor: pointer" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </template>
                            <template slot="footer">
                                <button class="btn btn-success btn-block btn-sm" :disabled="!canSave" v-on:click="save">Spara</button>
                            </template>
                        </modal>
                    </div>
                </div>
                <hr />
            </template>
            <ul class="list-unstyled list-spaced mb-0" v-if="!loading">
                <li><strong>Skytteliga</strong></li>
                <li class="ellipsis" v-for="item in goals" :key="`goals-${item.player.id}`">
                    <modal v-bind="{ attributes: { name: `goals-${item.player.id}` }, header: `${item.player.fullName}s mål`, button: { badge: 'badge-primary float-right', text: item.total } }">
                        <template slot="body">
                            <div v-for="event in item.events">
                                <span class="badge float-right" :class="{ 'badge-primary': event.amount > 0, 'badge-danger': event.amount <= 0 }">{{ event.amount }}</span>
                                <span class="mr-2">{{ getGameType(event.type) }}</span>
                            </div>
                        </template>
                    </modal>
                    {{ item.player.fullName }}
                </li>
                <hr />
                <li><strong>Assistliga</strong></li>
                <li class="ellipsis" v-for="item in assists" :key="`assists-${item.player.id}`">
                    <modal v-bind="{ attributes: { name: `assists-${item.player.id}` }, header: `${item.player.fullName}s assist`, button: { badge: 'badge-primary float-right', text: item.total } }">
                        <template slot="body">
                            <div v-for="event in item.events">
                                <span class="badge float-right" :class="{ 'badge-primary': event.amount > 0, 'badge-danger': event.amount <= 0 }">{{ event.amount }}</span>
                                <span class="mr-2">{{ getGameType(event.type) }}</span>
                            </div>
                        </template>
                    </modal>
                    {{ item.player.fullName }}
                </li>
                <hr />
                <li><strong>Gula kort</strong></li>
                <li class="ellipsis" v-for="item in yellowCards" :key="`yellowCards-${item.player.id}`">
                    <modal v-bind="{ attributes: { name: `yellowCards-${item.player.id}` }, header: `${item.player.fullName}s gula kort`, button: { badge: 'badge-primary float-right', text: item.total } }">
                        <template slot="body">
                            <div v-for="event in item.events">
                                <span class="badge float-right" :class="{ 'badge-primary': event.amount > 0, 'badge-danger': event.amount <= 0 }">{{ event.amount }}</span>
                                <span class="mr-2">{{ getGameType(event.type) }}</span>
                            </div>
                        </template>
                    </modal>
                    {{ item.player.fullName }}
                </li>
                <hr />
                <li><strong>Röda kort</strong></li>
                <li class="ellipsis" v-for="item in redCards" :key="`redCards-${item.player.id}`">
                    <modal v-bind="{ attributes: { name: `redCards-${item.player.id}` }, header: `${item.player.fullName}s röda kort`, button: { badge: 'badge-primary float-right', text: item.total } }">
                        <template slot="body">
                            <div v-for="event in item.events">
                                <span class="badge float-right" :class="{ 'badge-primary': event.amount > 0, 'badge-danger': event.amount <= 0 }">{{ event.amount }}</span>
                                <span class="mr-2">{{ getGameType(event.type) }}</span>
                            </div>
                        </template>
                    </modal>
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
import { Component, Watch } from 'vue-property-decorator'
import { Getter, Action, Mutation, State, namespace } from 'vuex-class'

import { PageViewModel } from '../model/common'
import { Game, GameType, PlayerEvent, GameEventType, Player } from '../model/squad'
import { Game as CrudGame, PlayerEvent as CrudPlayerEvent } from '../model/squad/crud'

const ModuleGetter = namespace('squad', Getter)
const ModuleAction = namespace('squad', Action)
const ModuleMutation = namespace('squad', Mutation)

import { GET_GAMES, GET_PLAYERS, GET_CRUD_GAME, FETCH_GAMES, CREATE_GAME, UPDATE_GAME, DELETE_GAME_EVENT, SET_CRUD_GAME } from '../modules/squad/types'

interface PlayerItem {
    id: number
    fullName: string
}

interface Event {
    player: PlayerItem
    gameType: GameType
    eventType: GameEventType
}

interface IPlayerEvent {
    type: GameType
    amount: number
}

interface ListItem {
    player: PlayerItem
    events: IPlayerEvent[]
    total: number
}

interface IGameEvent {
    event: CrudPlayerEvent
    component: VueComponent
}

import Modal from './modal.vue'
import FormComponent from './player-events-form.vue'
import DatePicker from 'vue-bootstrap-datetimepicker'

import { Component as VueComponent } from 'vue/types/options'

@Component({
    components: {
        Modal, DatePicker, FormComponent
    }
})
export default class PlayerEvents extends Vue {
    @State(state => state.vm) vm: PageViewModel
    @ModuleGetter(GET_GAMES) games: Game[]
    @ModuleGetter(GET_PLAYERS) players: Player[]
    @ModuleGetter(GET_CRUD_GAME) crudGame: CrudGame
    @ModuleAction(FETCH_GAMES) loadGames: () => Promise<void>
    @ModuleAction(CREATE_GAME) createGame: (payload: { game: CrudGame }) => Promise<void>
    @ModuleAction(UPDATE_GAME) updateGame: (payload: { game: CrudGame }) => Promise<void>
    @ModuleMutation(SET_CRUD_GAME) setCrudGame: (game: CrudGame) => void
    @ModuleMutation(DELETE_GAME_EVENT) deleteGameEvent: (event: CrudPlayerEvent) => void

    loading: boolean = false
    events: Event[] = []

    formComponents: IGameEvent[] = []

    gameType: string = ''

    selectedDate?: Date = new Date()

    selectedGameId: number = 0

    modalAttributes: any = {
		newEvent: {
			attributes: {
				name: 'new-event',
				scrollable: true
			},
			header: 'Matchhändelser',
			button: {
				classes: 'btn btn-sm btn-success btn-block float-right',
				text: 'Skapa ny match med händelser'
            },
            onClose: () => this.reset()
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

    $modal = (this as any).VModal

    mounted() {
        this.selectedDate = undefined
        this.fetchGames()
    }

    get loggedInUserIsAdminOrScissor(): boolean {
        return this.vm.loggedInUser.inRoles.some(role => role == 'Admin' || role == 'Scissor')
    }

    get goals() {
        return this.listify(GameEventType.Goal)
    }
    get assists() {
        return this.listify(GameEventType.Assist)
    }
    get redCards() {
        return this.listify(GameEventType.RedCard)
    }
    get yellowCards() {
        return this.listify(GameEventType.YellowCard)
    }

    get canSave() {
        return this.selectedDate 
            && this.gameType 
            && this.crudGame.events.length > 0
            && this.crudGame.events.filter((e: CrudPlayerEvent) => e.playerId == 0).length <= 0
    }
    get eventComponents() {
        return this.formComponents
    }

    @Watch('selectedGameId')
        changeGame() {
            let selectedGame: Game = this.games.filter((g: Game) => g.id == this.selectedGameId)[0]
            if (!selectedGame)
                return

            this.setCrudGame({
                id: selectedGame.id,
                type: selectedGame.type,
                playedDate: new Date(selectedGame.playedOn),
                events: [] // each events is set in player-events-form.vue component
            })

            this.gameType = GameType[selectedGame.type]
            this.selectedDate = new Date(selectedGame.playedOn)
            
            selectedGame.playerEvents.forEach((e: PlayerEvent) => {
                this.newEvent(undefined, {
                        playerId: e.player.id,
                        type: e.eventType
                    })
            })

            this.$modal.show(this.modalAttributes.newEvent.attributes.name)
        }

    reset() {
        this.setCrudGame(new CrudGame())
        this.gameType = ''
        this.selectedGameId = 0
        this.selectedDate = undefined
        this.formComponents = []
    }

    fetchGames() {
        this.loading = true
        this.loadGames().then(() => {
            this.events = []
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
            this.loading = false
        })
    }

    listify(type: GameEventType) {
        let items: ListItem[] = []
        let events = this.events.filter((e: Event) => e.eventType == type)
        events.forEach((e: Event) => {
            if (items.map((t: ListItem) => t.player.id).includes(e.player.id))
                return

            let allEvents: Event[] = events.filter((e2: Event) => e2.player.id == e.player.id)

            items.push({
                player: e.player,
                events: [
                    {
                        type: GameType.Cup,
                        amount: allEvents.filter((e: Event) => e.gameType == GameType.Cup).length
                    },
                    {
                        type: GameType.League,
                        amount: allEvents.filter((e: Event) => e.gameType == GameType.League).length
                    },
                    {
                        type: GameType.EuropaLeague,
                        amount: allEvents.filter((e: Event) => e.gameType == GameType.EuropaLeague).length
                    },
                    {
                        type: GameType.Training,
                        amount: allEvents.filter((e: Event) => e.gameType == GameType.Training).length
                    }
                ],
                total: allEvents.length
            })
        })
        items.forEach((item: ListItem) => {
            item.events.sort((a: IPlayerEvent, b: IPlayerEvent) => {
                return b.amount - a.amount
            })
        })
        items.sort((a: ListItem, b: ListItem) => {
            return b.total - a.total
        })
        return items
    }

    newEvent(e: any, event?: CrudPlayerEvent) {
        console.log('generate event component:', event)
        this.formComponents.push({
            event: event ? event : new CrudPlayerEvent(),
            component: FormComponent
        })
    }

    removeEvent(index: number, event: CrudPlayerEvent) {
        this.deleteGameEvent(event)
        this.formComponents.splice(index, 1)
    }

    save() {
        return new Promise<void>((resolve, reject) => {
            let game: CrudGame = {
                type: GameType[this.gameType as keyof typeof GameType],
                playedDate: this.selectedDate,
                events: this.crudGame.events
            }
            if (this.crudGame.id && this.crudGame.id > 0) {
                game.id = this.crudGame.id
                this.updateGame({ game: game }).then(() => resolve())
            } else {
                this.createGame({ game: game }).then(() => resolve())
            }
        }).then(() => {
            this.$modal.hide(this.modalAttributes.newEvent.attributes.name)
            this.fetchGames()
            
            this.gameType = ''
            this.selectedDate = undefined
            this.formComponents = []
        })
    }

    getGameType(type: GameType): string {
        switch (type) {
            case GameType.Cup:
                return 'Cupen'
            case GameType.League:
                return 'Allsvenskan'
            case GameType.EuropaLeague:
                return 'Europa League'
            case GameType.Training:
                return 'Träningsmatch'
            default:
                return ''
        }
    }
}
</script>
