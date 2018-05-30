<template>
    <div class="row">
        <div class="col p-0">
            <div class="card card-profile br br__none" style="margin-top: -1rem; margin-bottom: -1rem" v-if="!inEdit">
                <div class="card-header" style="background-image: url(/stadion.png);"></div>
                <div class="card-body text-center">
                    <ul class="card-menu mt-3">
                        <li class="card-menu-item">
                            Typ
                            <h6 class="my-0">{{ displayedGame.gameType }}</h6>
                        </li>
                        <li class="card-menu-item">
                            Motstånd
                            <h6 class="my-0">{{ displayedGame.opponent }}</h6>
                        </li>
                        <li class="card-menu-item">
                            Plats
                            <h6 class="my-0">{{ displayedGame.arenaType }}</h6>
                        </li>
                        <li class="card-menu-item">
                            Insläppta mål
                            <h6 class="my-0">{{ displayedGame.concededGoals }}</h6>
                        </li>
                        <li class="card-menu-item">
                            Datum
                            <h6 class="my-0">{{ displayedGame.date }}</h6>
                        </li>
                    </ul>
                </div>
                <div class="card-body pt-0">
                    <hr />
                    <ul class="list-unstyled mb-0" v-if="events.length > 0">
                        <li v-for="event in events">
                            <span class="icon icon-clock mr-2"></span> {{ event.inMinute }}' - <span v-html="event.text"></span>
                        </li>
                    </ul>
                    <div class="alert alert-info mb-0" v-else>
                        Hittade inga registrerade matchhändelser
                    </div>
                </div>
                <div class="card-footer" v-if="loggedInUserIsGameAdmin">
                    <span class="icon icon-edit float-right" style="cursor: pointer" @click="inEdit = true" v-tooltip="'Editera match'"></span>
                </div>
            </div>
            <div v-else>
                <div class="card br br__none" style="margin-top: -1rem; margin-bottom: -1rem">
                    <div class="card-body">
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
                                <div class="col-9">
                                    <date-picker v-model="selectedDate" :config="dpConfig" placeholder="Avspark" :class="{ 'form-control-sm': true }" />
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
                            <div class="row">
                                <legend class="col-sm-3 col-form-label pt-0"><strong>Tilläggstid</strong></legend>
                                <div class="col-sm-9">
                                    <input type="number" class="form-control form-control-sm" v-model="numberOfAddonMinutes" />
                                </div>
                            </div>
                        </div>
                            <div class="form-group">
                            <div class="row">
                                <legend class="col-sm-3 col-form-label pt-0"><strong>Tabellplacering</strong></legend>
                                <div class="col-sm-9">
                                    <input type="number" class="form-control form-control-sm" v-model="tablePlacementAfterGame" />
                                </div>
                            </div>
                        </div>
                        <div class="form-group">
                            <hr />
                            <strong>Startelvan</strong>
                            <div class="card mt-2" :class="{ 'br br__none': preDefinedLineup }">
                                <div class="card-body" :class="{ 'p-0': preDefinedLineup }">
                                    <lineups :lineup-type="'Real'" :pre-defined-lineup="preDefinedLineup" :show-create-button="false" />
                                </div>
                            </div>
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
                    <div class="card-footer">
                        <div class="row">
                            <div class="col" :class="{ 'pr-1': game }">
                                <button class="btn btn-success btn-block btn-sm" :disabled="!canSave" v-on:click="save">Spara</button>
                            </div>
                            <div class="col pl-1" v-if="game">
                                <button class="btn btn-danger btn-block btn-sm" v-on:click="inEdit = false">Avbryt</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import moment from 'moment'
import { Component, Watch } from 'vue-property-decorator'
import { Getter, Action, Mutation, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('squad', Getter)
const ModuleAction = namespace('squad', Action)
const ModuleMutation = namespace('squad', Mutation)

import { PageViewModel, Result, ResultType } from '../../../model/common'
import { Game, GameType, ArenaType, PlayerEvent, GameEventType, Player, PlayerToLineup, Lineup, LineupType } from '../../../model/squad'
import { Game as CrudGame, PlayerEvent as CrudPlayerEvent, Lineup as CrudLineup } from '../../../model/squad/crud'

import { Component as VueComponent } from 'vue/types/options'

interface IGame {
    id: number
    gameType: string
    opponent: string
    arenaType: string
    concededGoals: number
    date: string
    events: PlayerEvent[]
}

interface IEvent {
    inMinute: number
    text: string
}

interface IGameEvent {
    id: string
    event: CrudPlayerEvent
    component: VueComponent
}

import { GET_PLAYERS, GET_CRUD_GAME, GET_NEW_LINEUP, FETCH_SEASONS, CREATE_GAME, UPDATE_GAME, DELETE_GAME_EVENT, SET_CRUD_GAME, SET_GAME_EVENT, SET_GAME_EVENTS } from '../../../modules/squad/types'

import Modal from '../../../components/modal.vue'
import Results from '../../../components/results.vue'
import FormComponent from '../../../components/player-events-form.vue'
import Lineups from '../../../components/lineups/lineups.vue'

@Component({
    props: {
        game: Object
    },
	components: {
		Modal, Results, Lineups, FormComponent
	}
})
export default class PlayerComponent extends Vue {
    @State(state => state.vm) vm: PageViewModel
    @ModuleGetter(GET_PLAYERS) players: Player[]
    @ModuleGetter(GET_CRUD_GAME) crudGame: CrudGame
    @ModuleGetter(GET_NEW_LINEUP) newLineup: CrudLineup
    @ModuleAction(FETCH_SEASONS) loadSeasons: () => Promise<void>
    @ModuleAction(CREATE_GAME) createGame: (payload: { game: CrudGame }) => Promise<void>
    @ModuleAction(UPDATE_GAME) updateGame: (payload: { game: CrudGame }) => Promise<void>
    @ModuleMutation(SET_CRUD_GAME) setCrudGame: (game: CrudGame) => void
    @ModuleMutation(SET_GAME_EVENT) setGameEvent: (event: CrudPlayerEvent) => void
    @ModuleMutation(SET_GAME_EVENTS) setGameEvents: (events: CrudPlayerEvent[]) => void
    @ModuleMutation(DELETE_GAME_EVENT) deleteGameEvent: (event: CrudPlayerEvent) => void

    game: Game

    inEdit: boolean = true

    loading: boolean = false

    gameType: string = ''
    arenaType: string = ''

    selectedDate?: Date = new Date()

    selectedGameId: number = 0
    opponentTeamName: string = ''
    numberOfGoalsScoredByOpponent: number = 0
    numberOfAddonMinutes: number = 0
    tablePlacementAfterGame: number = 0
    preDefinedLineup?: Lineup = new Lineup()
    
    dpConfig: any = { 
		format: 'YYYY-MM-DD HH:mm', 
		useCurrent: false, 
		locale: 'sv', 
		icons: { 
			next: 'icon icon-chevron-right',
            previous: 'icon icon-chevron-left',
            time: "icon icon-clock",
            date: "icon icon-calendar",
            up: "icon icon-chevron-up",
            down: "icon icon-chevron-down"
        },
        widgetPositioning: {
            vertical: 'bottom',
            horizontal: 'left'
        }
    }

    $modal = (this as any).VModal

    mounted() {
        this.selectedDate = undefined
        if (this.game) {
            this.setGame(this.game)
            this.inEdit = false
        } else {
            this.setGame()
        }
    }

    get events() {
        if (this.game.playerEvents.length > 0) {
            let events = this.game.playerEvents.map((e: PlayerEvent) => {
                return <IEvent> {
                    inMinute: e.inMinute,
                    text: `<strong>${e.player.fullName}</strong> ${this.getEventType(e.eventType)}`
                }
            })
            events.push({ inMinute: 45, text: '<strong>HALVTID</strong>' })
            events.sort((a: IEvent, b: IEvent) => {
                return a.inMinute - b.inMinute
            })
            return events
        }
        return []
    }
    get loggedInUserIsGameAdmin(): boolean {
        return this.vm.loggedInUser.inRoles.some(role => role == 'Admin' || role == 'GameAdmin')
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
    get displayedGame() {
        if (this.game) {
            return <IGame> {
                id: this.game.id,
                gameType: this.getGameType(this.game.type),
                opponent: this.game.opponent,
                arenaType: this.getArenaType(this.game.arenaType),
                concededGoals: this.game.numberOfGoalsScoredByOpponent,
                date: this.game.playedOn,
                events: this.game.playerEvents
            }
        }
    }

    setGame(game?: Game) {
        if (!game) {
            this.reset()
            return
        }

        this.setCrudGame({
            id: game.id,
            type: game.type,
            arenaType: game.arenaType,
            lineup: game.lineup ? {
                id: game.lineup.id,
                formationId: game.lineup.formation.id,
                players: game.lineup.players.map((ptl: PlayerToLineup) => {
                    return {
                        playerId: ptl.player.id,
                        positionId: ptl.position.id
                    }
                }),
                createdByUserId: '',
                type: game.lineup.type
            } : undefined,
            opponent: game.opponent,
            numberOfGoalsScoredByOpponent: game.numberOfGoalsScoredByOpponent,
            numberOfAddonMinutes: game.numberOfAddonMinutes,
            tablePlacementAfterGame: game.tablePlacementAfterGame,
            playedDate: game.playedOn,
            events: game.playerEvents.map((e: PlayerEvent) => {
                return <CrudPlayerEvent> {
                    id: e.id,
                    playerId: e.player.id,
                    type: e.eventType,
                    inMinute: e.inMinute,

                    guid: (this as any).$helpers.guid()
                }
            })
        })

        this.gameType = GameType[game.type]
        this.selectedDate = new Date(game.playedOn)
        this.opponentTeamName = game.opponent
        this.arenaType = ArenaType[game.arenaType]
        this.preDefinedLineup = this.crudGame.lineup ? game.lineup : undefined
        this.numberOfGoalsScoredByOpponent = this.crudGame.numberOfGoalsScoredByOpponent
        this.numberOfAddonMinutes = this.crudGame.numberOfAddonMinutes
        this.tablePlacementAfterGame = this.crudGame.tablePlacementAfterGame
    }

    reset() {
        this.setCrudGame(new CrudGame())
        this.gameType = ''
        this.selectedGameId = 0
        this.selectedDate = undefined
        this.opponentTeamName = ''
        this.arenaType = ''
        this.preDefinedLineup = undefined
    }

    fetchSeasons() {
        this.loading = true
        return this.loadSeasons().then(() => this.loading = false)
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
                playedDate: this.selectedDate ? this.selectedDate.toLocaleString() : new Date().toLocaleString(),
                opponent: this.opponentTeamName,
                numberOfGoalsScoredByOpponent: this.numberOfGoalsScoredByOpponent,
                numberOfAddonMinutes: this.numberOfAddonMinutes,
                tablePlacementAfterGame: this.tablePlacementAfterGame,
                lineup: undefined,
                events: this.crudGame.events.map((e: CrudPlayerEvent) => {
                    delete e.guid
                    return e
                })
            }
            game.lineup = this.crudGame.lineup ? this.crudGame.lineup : this.newLineup.formationId > 0 && this.newLineup.players.length == 11 ? this.newLineup : undefined
            if (game.lineup) {
                game.lineup.createdByUserId = this.vm.loggedInUser.id
                game.lineup.type = LineupType.Real
            }
            if (this.crudGame.id && this.crudGame.id > 0) {
                game.id = this.crudGame.id
                this.updateGame({ game: game }).then(() => resolve())
            } else {
                this.createGame({ game: game }).then(() => resolve())
            }
        }).then(() => {
            this.$modal.hide(this.crudGame.id ? `show-game-${this.crudGame.id}` : 'new-game')
            this.fetchSeasons()
            
            this.gameType = ''
            this.selectedDate = undefined
        })
    }
    
    getEventType(type: GameEventType): string {
        switch (type) {
            case GameEventType.Goal:
                return 'gör mål'
            case GameEventType.Assist:
                return 'assisterar till mål'
            case GameEventType.YellowCard:
                return 'får gult kort'
            case GameEventType.RedCard:
                return 'får rött kort'
            case GameEventType.SubstituteIn:
                return 'blir inbytt'
            case GameEventType.SubstituteOut:
                return 'blir utbytt'
            default:
                return ''
        }
    }

    getGameType(type: GameType): string {
        switch (type) {
            case GameType.Cup:
                return 'Cupen'
            case GameType.League:
                return 'Allsvenskan'
            case GameType.EuropaLeague:
                return 'Europa league'
            case GameType.Training:
                return 'Träningsmatch'
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
a {
	cursor: pointer;
}
span.remove-event {
    cursor: pointer;
    color: black;
    font-size: 1.2rem;
    font-weight: bold;
}
</style>