<template>
    <div>
        <navbar />
        <div class="container pt-4 pb-5">
            <div class="row">
                <div class="col">
                    <ul class="list-group media-list media-list-stream">
                        <li class="list-group-item p-4">
                            <modal v-bind="{ attributes: { name: 'new-game', scrollable: true }, header: 'Ny match', button: { icon: 'icon icon-plus float-right', text: 'Skapa en ny match' } }" v-if="loggedInUserIsGameAdmin">
                                <template slot="body">
                                    <game-component />
                                </template>
                            </modal>
                            <h4 class="mb-0">Matcher</h4>
                        </li>
                        <li class="media list-group-item p-4">
                            <div class="col pl-0 pr-0">
                                <div class="form-check form-check-inline" v-for="season in seasons" :key="season.id">
                                    <input class="form-check-input" type="radio" v-model="selectedSeason" :id="season.id" :value="season.name">
                                    <label class="form-check-label" :for="season.id">{{ season.name }}</label>
                                </div>
                            </div>
                        </li>
                        <li class="media list-group-item p-4" v-if="!loading">
                            <table-component :data="games" sort-by="date" sort-order="desc" @rowClick="rowClick" :show-filter="false">
                                <table-column label="Id" :hidden="true" show="id"></table-column>
                                <table-column label="Typ" :hidden="true" show="gameType"></table-column>
                                <table-column label="Typ" sort-by="gameType" data-type="string">
                                    <template slot-scope="row">
                                        {{ row.gameType }}
                                        <span class="badge badge-danger ml-1" v-if="new Date(row.date) > new Date()">kommande</span>
                                    </template>
                                </table-column>
                                <table-column label="Motstånd" show="opponent"></table-column>
                                <table-column label="Plats" show="arenaType"></table-column>
                                <table-column label="Insläppta mål" show="concededGoals"></table-column>
                                <table-column label="Datum" show="date"></table-column>
                            </table-component>
                            <modal v-for="game in fullGames" :key="game.id" v-bind="{ attributes: { name: `show-game-${game.id}`, scrollable: true }, header: 'Matchinformation', button: { } }">
                                <template slot="body">
                                    <game-component :game="game" />
                                </template>
                            </modal>
                        </li>
                        <li class="media list-group-item p-4" v-if="!loading">
                            <div class="row" style="width: 100%">
                                <div class="col">
                                    <chartjs-line :option="options" :labels="labels" :datasets="chartData" />
                                </div>
                            </div>
                        </li> 
                        <li class="media list-group-item p-4" v-else>
                            <loader v-bind="{ background: '#699ED0' }" />
                        </li>
                    </ul>
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import moment from 'moment'
import { Component, Watch } from 'vue-property-decorator'
import { Getter, Action, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('squad', Getter)
const ModuleAction = namespace('squad', Action)

import { PageViewModel, Result, ResultType } from '../../../model/common'
import { Player, Position, PlayerEventOnPlayer, GameType, GameEventType, PlayerEvent, Season, ArenaType, Game } from '../../../model/squad'
import { Player as CrudPlayer } from '../../../model/squad/crud'

import {
    GET_SEASONS,
    FETCH_SEASONS
} from '../../../modules/squad/types'

import Modal from '../../../components/modal.vue'
import Results from '../../../components/results.vue'
import GameComponent from './game.vue'

interface IGame {
    id: number
    gameType: string
    opponent: string
    arenaType: string
    concededGoals: number
    date: string
    events: PlayerEvent[]
}

@Component({
	components: {
		Modal, Results, GameComponent
	}
})
export default class GamesComponent extends Vue {
    @State(state => state.vm) vm: PageViewModel
	@ModuleGetter(GET_SEASONS) seasons: Season[]
    @ModuleAction(FETCH_SEASONS) loadSeasons: () => Promise<void>
    
    loading: boolean = false

    selectedSeason: string = ''

    today: Date = new Date()

    labels: string[] = []

    colors: string[] = ['#0568AF', '#E21F26', '#699ED0', '#E0A541']

    options: any = {
        title: {
            display: true,
            position: 'bottom',
            text: 'Tabellutveckling i Allsvenskan'
        },
        scales: {
            yAxes: [{
                ticks: {
                    max: 16,
                    min: 1,
                    stepSize: 1,
                    reverse: true,
                    beginAtZero: false
                }
            }]
        }
    };

	$modal: any = (this as any).VModal
    
    created() {
        this.loading = true
        this.loadSeasons().then(() => {
            this.loading = false
            this.selectedSeason = this.seasons.filter((s: Season) => s.isActive)[0].name
        })

        for (let i = 1; i <= 30; i++) {
            this.labels.push(`Omgång ${i}`)
        }
    }

    get seasonGames() {
        return this.seasons.filter((s: Season) => s.name == this.selectedSeason).map((s: Season) => s.games)[0]
    }

    get games() {
        if (this.seasonGames) {
            return this.seasonGames.map((g: Game) => {
                return <IGame> {
                    id: g.id,
                    gameType: this.getGameType(g.type),
                    opponent: g.opponent,
                    arenaType: this.getArenaType(g.arenaType),
                    concededGoals: g.numberOfGoalsScoredByOpponent,
                    date: g.playedOn,
                    events: g.playerEvents
                }
            })
        }
    }
    get fullGames() {
        if (this.seasonGames) {
            return this.seasonGames
        }
    }
    get loggedInUserIsGameAdmin(): boolean {
        return this.vm.loggedInUser.inRoles.some(role => role == 'Admin' || role == 'GameAdmin')
    }
    get chartData() {
        return this.seasons.map((s: Season, index: number) => {
            let color: string = this.colors[index]
            return {
                label: s.name,
                data: s.games.filter((g: Game) => g.type == GameType.League && g.tablePlacementAfterGame > 0).map((g: Game) => g.tablePlacementAfterGame),
                borderColor: color,
                backgroundColor: color,
                fill: false,
                lineTension: 0.1,
                pointBorderColor: color,
                pointBackgroundColor: "#fff",
                pointBorderWidth: 1,
                pointHoverRadius: 5,
                pointHoverBackgroundColor: color,
                pointHoverBorderColor: "rgba(220,220,220,1)",
                pointHoverBorderWidth: 2,
                pointRadius: 1,
                pointHitRadius: 10,
                spanGaps: false
            }
        })
    }

    rowClick(row: any) {
        this.$modal.show(`show-game-${row.data.id}`)        
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
</style>