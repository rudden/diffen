<template>
    <div class="card mb-4">
        <div class="card-body">
            <template v-if="header">
                <h6>{{ header }}</h6>
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
import { Game, GameType, ArenaType, PlayerEvent, GameEventType, Player, PlayerToLineup, Lineup, LineupType } from '../model/squad'
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
    inMinute: number
    opponent: string
    gameType: GameType
    eventType: GameEventType
    arenaType: ArenaType
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

    $modal = (this as any).VModal

    mounted() {
        this.fetchGames()
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
    get gameEvents() {
        return this.crudGame.events.map((e: CrudPlayerEvent) => {
            return <IGameEvent> {
                event: e,
                component: FormComponent
            }
        })
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
                        inMinute: playerEvent.inMinute,
                        gameType: game.type,
                        opponent: game.opponent,
                        eventType: playerEvent.eventType,
                        arenaType: game.arenaType
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
                    this.getListifiedItems(allEvents, GameType.Cup),
                    this.getListifiedItems(allEvents, GameType.League),
                    this.getListifiedItems(allEvents, GameType.EuropaLeague),
                    this.getListifiedItems(allEvents, GameType.Training)
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

    getListifiedItems(events: Event[], gameType: GameType) {
        let games = events.filter((event: Event) => event.gameType == gameType)
        return <IPlayerEvent> {
            type: GameType.Cup,
            dates: games.map((e: Event) => `${e.opponent} (${this.getShortArenaType(e.arenaType)}) - (${e.inMinute}')`),
            amount: games.length
        }
    }

    newEvent(e: any) {
        this.setGameEvent(new CrudPlayerEvent((this as any).$helpers.guid()))
    }

    removeEvent(guid: string) {
        this.deleteGameEvent(this.crudGame.events.filter((e: CrudPlayerEvent) => e.guid == guid)[0])
    }

    getShortArenaType(arenaType: ArenaType) {
        switch (arenaType) {
            case ArenaType.Home:
                return 'h'
            case ArenaType.Away:
                return 'b'
            case ArenaType.NeutralGround:
                return 'n'
        }
    }
}
</script>