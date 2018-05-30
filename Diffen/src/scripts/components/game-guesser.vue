<template>
    <div :class="{ 'card mb-4': !guesserOnly }">
        <div :class="{ 'card-body': !guesserOnly }">
            <template v-if="!guesserOnly">
                <small v-if="!loading && upcomingGame" class="float-right text-muted">
                    <span class="icon icon-clock"></span>
                    {{ upcomingGame.playedOn }}
                </small>
                <h6>Tippa nästa match</h6>
                <hr />
            </template>
            <template v-if="!loading">
                <template v-if="upcomingGame">
                    <div class="row">
                        <div class="col pr-1">
                            <strong>{{ difPlaysAtHome ? 'DIF' : upcomingGame.opponent }}</strong>
                            <template v-if="guess">
                                <div class="alert alert-primary p-1 mb-0">
                                    <strong>{{ difPlaysAtHome ? guess.difGoals : guess.opponentGoals }} mål</strong>
                                </div>
                            </template>
                            <template v-else>
                                <template v-if="difPlaysAtHome">
                                    <select class="form-control form-control-sm" v-model="selectedNumberOfDifGoals">
                                        <option v-for="goal in goals" :value="goal" :key="goal">{{ goal }}</option>
                                    </select>
                                </template>
                                <template v-else>
                                    <select class="form-control form-control-sm" v-model="selectedNumberOfOpponentGoals">
                                        <option v-for="goal in goals" :value="goal" :key="goal">{{ goal }}</option>
                                    </select>
                                </template>
                            </template>
                        </div>
                        <div class="col pl-1">
                            <strong>{{ !difPlaysAtHome ? 'DIF' : upcomingGame.opponent }}</strong>
                            <template v-if="guess">
                                <div class="alert alert-primary p-1 mb-0">
                                    <strong>{{ !difPlaysAtHome ? guess.difGoals : guess.opponentGoals }} mål</strong>
                                </div>
                            </template>
                            <template v-else>
                                <template v-if="!difPlaysAtHome">
                                    <select class="form-control form-control-sm" v-model="selectedNumberOfDifGoals">
                                        <option v-for="goal in goals" :value="goal" :key="goal">{{ goal }}</option>
                                    </select>
                                </template>
                                <template v-else>
                                    <select class="form-control form-control-sm" v-model="selectedNumberOfOpponentGoals">
                                        <option v-for="goal in goals" :value="goal" :key="goal">{{ goal }}</option>
                                    </select>
                                </template>
                            </template>
                        </div>
                    </div>
                    <div class="row mt-3">
                        <div class="col pr-1" v-if="!guess">
                            <button class="btn btn-sm btn-success btn-block" @click="createGuessGameResult">Spara</button>
                        </div>
                        <div class="col" :class="{ 'pl-1': !guess }">
                            <modal v-bind="{ attributes: { name: 'guess-league', scrollable: true }, button: { classes: 'btn btn-sm btn-block btn-primary', text: 'Visa tipsligan' } }">
                                <template slot="body">
                                    <game-result-guess-league />
                                </template>
                            </modal>
                        </div>
                    </div>
                </template>
                <template v-else>
                    <div class="alert alert-warning mb-0">Ingen kommande match</div>
                </template>
            </template>
            <template v-else>
                <loader v-bind="{ background: '#699ED0' }" />
            </template>
        </div>
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Action, Mutation, State, namespace } from 'vuex-class'

import { PageViewModel } from '../model/common'

const ModuleAction = namespace('squad', Action)

import { GUESS_GAME_RESULT, FETCH_UPCOMING_GAME } from '../modules/squad/types'

import { Game, GameResultGuess, ArenaType } from '../model/squad'
import { GameResultGuess as CrudResultGuess } from '../model/squad/crud'

interface IGameResultGuess {
    difGoals: number
    opponentGoals: number
}

import Modal from './modal.vue'
import GameResultGuessLeague from './game-result-guess-league.vue'

@Component({
    props: {
        guesserOnly: {
            type: Boolean,
            default: false
        }
    },
    components: {
        GameResultGuessLeague, Modal
    }
})
export default class GameGuesser extends Vue {
    @State(state => state.vm) vm: PageViewModel
    @ModuleAction(FETCH_UPCOMING_GAME) loadUpcomingGame: () => Promise<Game>
    @ModuleAction(GUESS_GAME_RESULT) guessGameResult: (payload: { guess: CrudResultGuess }) => Promise<void>

    guesserOnly: boolean

    loading: boolean = false
    upcomingGame: Game = new Game()
    selectedNumberOfDifGoals: number = 0
    selectedNumberOfOpponentGoals: number = 0

    mounted() {
        this.loading = true
        this.loadUpcomingGame()
            .then((game: Game) => {
                this.upcomingGame = game
                if (this.guess) {
                    this.selectedNumberOfDifGoals = this.guess.difGoals
                    this.selectedNumberOfOpponentGoals = this.guess.opponentGoals
                }
                this.loading = false
            })
    }

    get user() {
        return this.vm.loggedInUser
    }
    get goals() {
        return [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
    }
    get guess() {
        var gameGuess = this.vm.loggedInUser.gameResultGuesses.filter((guess: GameResultGuess) => guess.game.id == this.upcomingGame.id)[0]
        if (gameGuess) {
            return {
                difGoals: gameGuess.numberOfGoalsScoredByDif,
                opponentGoals: gameGuess.numberOfGoalsScoredByOpponent
            }
        }
        return undefined
    }
    get difPlaysAtHome() {
        return this.upcomingGame.arenaType == ArenaType.Home ? true : false
    }

    createGuessGameResult() {
        this.loading = true
        this.guessGameResult({ guess: {
                gameId: this.upcomingGame.id,
                numberOfGoalsScoredByDif: this.selectedNumberOfDifGoals,
                numberOfGoalsScoredByOpponent: this.selectedNumberOfOpponentGoals,
                guessedByUserId: this.vm.loggedInUser.id
            }
        }).then(() => this.loading = false)
    }
}
</script>
