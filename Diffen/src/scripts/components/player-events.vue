<template>
    <div class="card mb-4">
        <div class="card-body">
            <template v-if="header">
                <h6>{{ header }}</h6>
                <hr />
            </template>
            <template v-if="loggedInUserIsAdminOrScissor && !isSmall">
                <div class="row">
                    <div class="col-10 pr-0">
                        <div class="form-group mb-0">
                            <select class="form-control form-control-sm" v-model="selectedGameId">
                                <option value="0">Hantera händelser i en match</option>
                                <option v-for="game in games" :value="game.id">{{ getGameType(game.type) }} - {{ game.playedOn }}</option>
                            </select>
                        </div>
                    </div>
                    <div class="col-2 mt-1">
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
                <events-list :header="'Skytteliga'" :items="goals" :type-name="'gjorda mål'" :is-small="isSmall" />
                <hr />
                <events-list :header="'Assistliga'" :items="assists" :type-name="'assister'" :is-small="isSmall" />
                <hr />
                <events-list :header="'Gula kort'" :items="yellowCards" :type-name="'gula kort'" :is-small="isSmall" />
                <hr />
                <events-list :header="'Röda kort'" :items="redCards" :type-name="'röda kort'" :is-small="isSmall" />
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
    playedOn: string
    gameType: GameType
    eventType: GameEventType
}

interface IPlayerEvent {
    type: GameType
    dates: string[]
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
import EventsList from './player-events-list.vue'
import DatePicker from 'vue-bootstrap-datetimepicker'

import { Component as VueComponent } from 'vue/types/options'

@Component({
    props: {
        defaultGameType: {
            type: String,
            default: undefined
        },
        isSmall: {
            type: Boolean,
            default: false
        },
        header: {
            type: String,
            default: undefined
        }
    },
    components: {
        Modal, DatePicker, FormComponent, EventsList
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

    defaultGameType: string
    isSmall: boolean
    header: string

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
				icon: 'icon icon-plus float-right',
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
                if (this.defaultGameType) {
                    if (game.type !== GameType[this.defaultGameType as keyof typeof GameType]) {
                        return
                    }
                }

                game.playerEvents.forEach((playerEvent: PlayerEvent) => {
                    this.events.push({
                        player: {
                            id: playerEvent.player.id,
                            fullName: playerEvent.player.fullName,
                        },
                        playedOn: game.playedOn,
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

            let cupGames = allEvents.filter((e: Event) => e.gameType == GameType.League)
            let leagueGames = allEvents.filter((e: Event) => e.gameType == GameType.League)
            let europeLeagueGames = allEvents.filter((e: Event) => e.gameType == GameType.EuropaLeague)
            let trainingGames = allEvents.filter((e: Event) => e.gameType == GameType.Training)

            items.push({
                player: e.player,
                events: [
                    {
                        type: GameType.Cup,
                        dates: cupGames.map((e: Event) => e.playedOn),
                        amount: cupGames.length
                    },
                    {
                        type: GameType.League,
                        dates: leagueGames.map((e: Event) => e.playedOn),
                        amount: leagueGames.length
                    },
                    {
                        type: GameType.EuropaLeague,
                        dates: europeLeagueGames.map((e: Event) => e.playedOn),
                        amount: europeLeagueGames.length
                    },
                    {
                        type: GameType.Training,
                        dates: trainingGames.map((e: Event) => e.playedOn),
                        amount: trainingGames.length
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
