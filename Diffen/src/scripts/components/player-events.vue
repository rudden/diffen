<template>
    <div class="card mb-4">
        <div class="card-body">
            <template v-if="header">
                <h6>{{ header }}</h6>
                <hr />
            </template>
            <template v-if="loggedInUserIsGameAdmin && !isSmall">
                <div class="row">
                    <div class="col-10 pr-0">
                        <div class="form-group mb-0">
                            <select class="form-control form-control-sm" v-model="selectedGameId">
                                <option value="0">Hantera händelser i en match</option>
                                <option v-for="game in games" :value="game.id" :key="game.id">
                                    {{ game.playedOn }} ({{ getGameType(game.type) }}) - {{ game.opponent }} ({{ getArenaType(game.arenaType)}})
                                </option>
                            </select>
                        </div>
                    </div>
                    <div class="col-2 mt-1">
                        <modal v-bind="modalAttributes.newEvent">
                            <template slot="body">
                                <div class="row">
                                    <div class="col">
                                        <div class="form-group">
                                            <div class="row">
                                                <legend class="col-sm-3 col-form-label pt-0"><strong>Motståndare</strong></legend>
                                                <div class="col-sm-9">
                                                    <input type="text" class="form-control form-control-sm" v-model="opponentTeamName" placeholder="Namnet på motståndarlaget">
                                                </div>
                                            </div>
                                        </div>
                                        <fieldset class="form-group">
                                            <div class="row">
                                                <legend class="col-sm-3 col-form-label pt-0"><strong>Typ av match</strong></legend>
                                                <div class="col-sm-9">
                                                    <div class="form-check form-check-inline">
                                                        <input class="form-check-input" type="radio" id="cup" v-model="gameType" value="Cup" />
                                                        <label class="form-check-label" for="cup">Cup</label>
                                                    </div>
                                                    <div class="form-check form-check-inline">
                                                        <input class="form-check-input" type="radio" id="league" v-model="gameType" value="League" />
                                                        <label class="form-check-label" for="league">Allsvenskan</label>
                                                    </div>
                                                    <div class="form-check form-check-inline">
                                                        <input class="form-check-input" type="radio" id="training" v-model="gameType" value="Training" />
                                                        <label class="form-check-label" for="training">Träningsmatch</label>
                                                    </div>
                                                    <div class="form-check form-check-inline">
                                                        <input class="form-check-input" type="radio" id="europaLeague" v-model="gameType" value="EuropaLeague" />
                                                        <label class="form-check-label" for="europaLeague">Europa League</label>
                                                    </div>
                                                </div>
                                            </div>
                                        </fieldset>
                                        <fieldset class="form-group">
                                            <div class="row">
                                                <legend class="col-sm-3 col-form-label pt-0"><strong>Plats</strong></legend>
                                                <div class="col-sm-9">
                                                    <div class="form-check form-check-inline">
                                                        <input class="form-check-input" type="radio" id="home" v-model="arenaType" value="Home" />
                                                        <label class="form-check-label" for="home">Hemma</label>
                                                    </div>
                                                    <div class="form-check form-check-inline">
                                                        <input class="form-check-input" type="radio" id="away" v-model="arenaType" value="Away" />
                                                        <label class="form-check-label" for="away">Borta</label>
                                                    </div>
                                                    <div class="form-check form-check-inline">
                                                        <input class="form-check-input" type="radio" id="neutral" v-model="arenaType" value="NeutralGround" />
                                                        <label class="form-check-label" for="neutral">Neutral mark</label>
                                                    </div>
                                                </div>
                                            </div>
                                        </fieldset>
                                        <div class="form-group">
                                            <div class="row">
                                                <legend class="col-sm-3 col-form-label pt-0"><strong>Datum</strong></legend>
                                                <div class="col-sm-9">
                                                    <v-datepicker v-model="selectedDate" :format="'yyyy-MM-dd'" :bootstrap-styling="true" :input-class="'form-control-sm'" :monday-first="true" :placeholder="'Välj datum'" />
                                                </div>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <div class="row">
                                                <legend class="col-sm-3 col-form-label pt-0"><strong>Moståndarmål</strong></legend>
                                                <div class="col-sm-9">
                                                    <input type="number" class="form-control form-control-sm" v-model="numberOfGoalsScoredByOpponent" />
                                                </div>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <lineups :header="'Startelvan'" :lineup-type="'Real'" :pre-selected-lineup-id="preSelectedLineupId" />
                                        </div>
                                        <div class="form-group">
                                            <hr />
                                            <span class="icon icon-plus float-right" style="cursor: pointer" v-on:click="newEvent" v-tooltip="'Ny händelse'"></span>
                                            <h6>Händelser</h6>
                                        </div>
                                        <div class="form-group mb-0">
                                            <ul class="list-group">
                                                <li class="list-group-item" v-for="(item, index) in gameEvents" :key="index">
                                                    <div class="row">
                                                        <div class="col-11 pr-0">
                                                            <component :is="item.component" v-bind="{ event: item.event }" :key="item.event.guid" />
                                                        </div>
                                                        <div class="col-1 pl-0">
                                                            <span v-on:click="removeEvent(item.event.guid)" class="float-right remove-event" v-tooltip="'Ta bort händelse'">&times;</span>
                                                        </div>
                                                    </div>
                                                </li>
                                            </ul>
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
import { Game, GameType, ArenaType, PlayerEvent, GameEventType, Player, PlayerToLineup } from '../model/squad'
import { Game as CrudGame, PlayerEvent as CrudPlayerEvent, Lineup as CrudLineup } from '../model/squad/crud'

const ModuleGetter = namespace('squad', Getter)
const ModuleAction = namespace('squad', Action)
const ModuleMutation = namespace('squad', Mutation)

import { GET_GAMES, GET_PLAYERS, GET_CRUD_GAME, GET_NEW_LINEUP, FETCH_GAMES, CREATE_GAME, UPDATE_GAME, DELETE_GAME_EVENT, SET_CRUD_GAME, SET_GAME_EVENT, SET_GAME_EVENTS } from '../modules/squad/types'

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
    id: string
    event: CrudPlayerEvent
    component: VueComponent
}

import Modal from './modal.vue'
import FormComponent from './player-events-form.vue'
import EventsList from './player-events-list.vue'
import Lineups from './lineups/lineups.vue'

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
        Modal, FormComponent, EventsList, Lineups
    }
})
export default class PlayerEvents extends Vue {
    @State(state => state.vm) vm: PageViewModel
    @ModuleGetter(GET_GAMES) games: Game[]
    @ModuleGetter(GET_PLAYERS) players: Player[]
    @ModuleGetter(GET_CRUD_GAME) crudGame: CrudGame
	@ModuleGetter(GET_NEW_LINEUP) newLineup: CrudLineup
    @ModuleAction(FETCH_GAMES) loadGames: () => Promise<void>
    @ModuleAction(CREATE_GAME) createGame: (payload: { game: CrudGame }) => Promise<void>
    @ModuleAction(UPDATE_GAME) updateGame: (payload: { game: CrudGame }) => Promise<void>
    @ModuleMutation(SET_CRUD_GAME) setCrudGame: (game: CrudGame) => void
    @ModuleMutation(SET_GAME_EVENT) setGameEvent: (event: CrudPlayerEvent) => void
    @ModuleMutation(SET_GAME_EVENTS) setGameEvents: (events: CrudPlayerEvent[]) => void
    @ModuleMutation(DELETE_GAME_EVENT) deleteGameEvent: (event: CrudPlayerEvent) => void

    defaultGameType: string
    isSmall: boolean
    header: string

    loading: boolean = false
    events: Event[] = []

    gameType: string = ''
    arenaType: string = ''

    selectedDate?: Date = new Date()

    selectedGameId: number = 0
    opponentTeamName: string = ''
    numberOfGoalsScoredByOpponent: number = 0
    preSelectedLineupId: number = 0

    modalAttributes: any = {
		newEvent: {
			attributes: {
				name: 'new-event',
				scrollable: true
			},
			header: 'Match',
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

    get loggedInUserIsGameAdmin(): boolean {
        return this.vm.loggedInUser.inRoles.some(role => role == 'Admin' || role == 'GameAdmin')
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
    get substitutesIn() {
        return this.listify(GameEventType.SubstituteIn)
    }
    get substitutesOut() {
        return this.listify(GameEventType.SubstituteOut)
    }
    get canSave() {
        return this.selectedDate 
            && this.opponentTeamName
            && this.gameType 
            && this.arenaType
            && (this.newLineup.formationId > 0 && this.newLineup.players.length < 11 ? false : true)
            && this.crudGame.events.filter((e: CrudPlayerEvent) => e.playerId == 0).length <= 0
            && this.crudGame.events.filter((e: CrudPlayerEvent) => e.inMinute == 0).length <= 0
    }
    get gameEvents() {
        return this.crudGame.events.map((e: CrudPlayerEvent) => {
            return <IGameEvent> {
                event: e,
                component: FormComponent
            }
        })
    }

    @Watch('selectedGameId')
        changeGame() {
            let selectedGame: Game = this.games.filter((g: Game) => g.id == this.selectedGameId)[0]
            if (!selectedGame)
                return

            this.setCrudGame({
                id: selectedGame.id,
                type: selectedGame.type,
                arenaType: selectedGame.arenaType,
                lineup: selectedGame.lineup ? {
                    id: selectedGame.lineup.id,
                    formationId: selectedGame.lineup.formation.id,
                    players: selectedGame.lineup.players.map((ptl: PlayerToLineup) => {
                        return {
                            playerId: ptl.player.id,
                            positionId: ptl.position.id
                        }
                    }),
                    createdByUserId: '',
                    type: selectedGame.lineup.type
                } : undefined,
                opponent: selectedGame.opponent,
                numberOfGoalsScoredByOpponent: selectedGame.numberOfGoalsScoredByOpponent,
                playedDate: new Date(selectedGame.playedOn),
                events: selectedGame.playerEvents.map((e: PlayerEvent) => {
                    return <CrudPlayerEvent> {
                        id: e.id,
                        playerId: e.player.id,
                        type: e.eventType,
                        inMinute: e.inMinute,

                        guid: (this as any).$helpers.guid()
                    }
                })
            })

            this.gameType = GameType[selectedGame.type]
            this.selectedDate = new Date(selectedGame.playedOn)
            this.opponentTeamName = selectedGame.opponent
            this.arenaType = ArenaType[selectedGame.arenaType]
            this.preSelectedLineupId = this.crudGame.lineup ? <number>this.crudGame.lineup.id : 0
            this.numberOfGoalsScoredByOpponent = this.crudGame.numberOfGoalsScoredByOpponent

            this.$modal.show(this.modalAttributes.newEvent.attributes.name)
        }

    reset() {
        this.setCrudGame(new CrudGame())
        this.gameType = ''
        this.selectedGameId = 0
        this.selectedDate = undefined
        this.opponentTeamName = ''
        this.arenaType = ''
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

            let cupGames = allEvents.filter((e: Event) => e.gameType == GameType.Cup)
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

    newEvent(e: any) {
        this.setGameEvent(new CrudPlayerEvent((this as any).$helpers.guid()))
    }

    removeEvent(guid: string) {
        this.deleteGameEvent(this.crudGame.events.filter((e: CrudPlayerEvent) => e.guid == guid)[0])
    }

    save() {
        return new Promise<void>((resolve, reject) => {
            let game: CrudGame = {
                type: GameType[this.gameType as keyof typeof GameType],
                arenaType: ArenaType[this.arenaType as keyof typeof ArenaType],
                playedDate: this.selectedDate,
                opponent: this.opponentTeamName,
                numberOfGoalsScoredByOpponent: this.numberOfGoalsScoredByOpponent,
                lineup: undefined,
                events: this.crudGame.events.map((e: CrudPlayerEvent) => {
                    delete e.guid
                    return e
                })
            }
            if (this.crudGame.id && this.crudGame.id > 0) {
                game.id = this.crudGame.id
                game.lineup = this.crudGame.lineup ? this.crudGame.lineup : this.newLineup.formationId > 0 && this.newLineup.players.length == 11 ? this.newLineup : undefined
                if (game.lineup) {
                    game.lineup.createdByUserId = this.vm.loggedInUser.id
                }
                this.updateGame({ game: game }).then(() => resolve())
            } else {
                game.lineup = this.newLineup
                game.lineup.createdByUserId = this.vm.loggedInUser.id
                this.createGame({ game: game }).then(() => resolve())
            }
        }).then(() => {
            this.$modal.hide(this.modalAttributes.newEvent.attributes.name)
            this.fetchGames()
            
            this.gameType = ''
            this.selectedDate = undefined
        })
    }

    getGameType(type: GameType): string {
        switch (type) {
            case GameType.Cup:
                return 'Cup'
            case GameType.League:
                return 'AS'
            case GameType.EuropaLeague:
                return 'EL'
            case GameType.Training:
                return 'Träning'
            default:
                return ''
        }
    }

    
    getArenaType(type: ArenaType): string {
        switch (type) {
            case ArenaType.Home:
                return 'Hemma'
            case ArenaType.Away:
                return 'Borta'
            case ArenaType.NeutralGround:
                return 'Neutral'
            default:
                return ''
        }
    }
}
</script>

<style lang="scss" scoped>
span.remove-event {
    cursor: pointer;
    color: black;
    font-size: 1.2rem;
    font-weight: bold;
}
</style>
