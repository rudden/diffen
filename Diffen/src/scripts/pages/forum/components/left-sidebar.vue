<template>
    <div class="col-lg-3 d-none d-lg-block" v-if="showLeftSideBar">
        <div class="card card-profile mb-4">
            <div class="card-header" style="background-image: url(/bg.jpg);"></div>
            <div class="card-body text-center">
                <a href="/profil">
                    <img class="card-profile-img mb-3" :src="user.avatar">
                </a>
                <p class="mb-4" v-if="user.bio">{{ user.bio }}</p>
                <ul class="card-menu mb-0">
                    <li class="card-menu-item">
                        Inlägg
                        <h6 class="my-0">{{ user.numberOfPosts }}</h6>
                    </li>
                    <li class="card-menu-item">
                        Karma
                        <h6 class="my-0">{{ user.karma }}</h6>
                    </li>
                </ul>
            </div>
        </div>
        <div class="card mb-4">
            <div class="card-body">
                <h6 class="card-title">
                    <a class="text-inherit" href="/profil">{{ user.nickName }}</a>
                </h6>
                <hr />
                <ul class="list-unstyled list-spaced mb-0">
                    <li><span class="text-muted icon icon-thumbs-up mr-3"></span>{{ user.voteStatistics.upVotes }} givna upptummar</li>
                    <li><span class="text-muted icon icon-thumbs-down mr-3"></span>{{ user.voteStatistics.downVotes }} givna nertummar</li>
                    <template v-if="user.savedPostsIds">
                        <li><span class="text-muted icon icon-bookmark mr-3"></span>{{ user.savedPostsIds.length }} sparade inlägg</li>
                    </template>
                    <template v-if="user.favoritePlayer">
                        <li><span class="text-muted icon icon-heart mr-3"></span>{{ user.favoritePlayer.fullName }}</li>
                    </template>
                    <template v-if="user.region">
                        <li class="text-uppercase"><span class="text-muted icon icon-globe mr-3"></span>DIF {{ user.region }}</li>
                    </template>
                </ul>
            </div>
        </div>
        <div class="card mb-4">
            <div class="card-body">
                <h6>Tippa nästa match</h6>
                <hr />
                <template v-if="!creatingGuess">
                    <div class="row">
                        <div class="col pr-1">
                            <template v-if="loggedInUsersGameResultGuess">
                                <strong>DIF</strong>
                                <div class="alert alert-primary p-1 mb-0">
                                    <strong>{{ loggedInUsersGameResultGuess.difGoals }} mål</strong>
                                </div>
                            </template>
                            <template v-else>
                                <strong>DIF</strong>
                                <select class="form-control form-control-sm" v-model="selectedNumberOfDifGoals">
                                    <option v-for="goal in goals" :value="goal">{{ goal }}</option>
                                </select>
                            </template>
                        </div>
                        <div class="col pl-1">
                            <template v-if="loggedInUsersGameResultGuess">
                                <strong>{{ upcomingGame.opponent }}</strong>
                                <div class="alert alert-primary p-1 mb-0">
                                    <strong>{{ loggedInUsersGameResultGuess.opponentGoals }} mål</strong>
                                </div>
                            </template>
                            <template v-else>
                                <strong>{{ upcomingGame.opponent }}</strong>
                                <select class="form-control form-control-sm" v-model="selectedNumberOfOpponentGoals" :readonly="loggedInUsersGameResultGuess">
                                    <option v-for="goal in goals" :value="goal">{{ goal }}</option>
                                </select>
                            </template>
                        </div>
                    </div>
                    <div class="row mt-3" v-if="!loggedInUsersGameResultGuess">
                        <div class="col">
                            <button class="btn btn-sm btn-success btn-block" @click="createGuessGameResult">Spara</button>
                        </div>
                    </div>
                </template>
                <template v-else>
                    <loader v-bind="{ background: '#699ED0' }" />
                </template>
            </div>
        </div>
        <rss-feed :url="'https://www.jarnkaminerna.se/feed/'" :amount="5" :feed-name="'Senaste från Järnkaminerna'" />
		<rss-feed :url="'https://diftv.solidtango.com/feed/'" :amount="5" :feed-name="'Senaste från DIFTV'" />
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Action, Mutation, State, namespace } from 'vuex-class'

import { PageViewModel } from '../../../model/common'

const ModuleGetter = namespace('forum', Getter)
const ModuleAction = namespace('forum', Action)
const ModuleMutation = namespace('forum', Mutation)

const SquadModuleAction = namespace('squad', Action)

import { GET_SHOW_LEFT_SIDEBAR, SET_SHOW_LEFT_SIDEBAR } from '../../../modules/forum/types'
import { GUESS_GAME_RESULT, FETCH_UPCOMING_GAME } from '../../../modules/squad/types'

import { Game, GameResultGuess } from '../../../model/squad'
import { GameResultGuess as CrudResultGuess } from '../../../model/squad/crud'

import RssFeed from '../../../components/rss.vue'

interface IGameResultGuess {
    difGoals: number
    opponentGoals: number
}

@Component({
    components: {
        RssFeed
    }
})
export default class LeftSidebar extends Vue {
    @State(state => state.vm) vm: PageViewModel
    @ModuleGetter(GET_SHOW_LEFT_SIDEBAR) showLeftSideBar: boolean
    @ModuleMutation(SET_SHOW_LEFT_SIDEBAR) setShowLeftSideBar: (payload: { value: boolean }) => void
    @SquadModuleAction(FETCH_UPCOMING_GAME) loadUpcomingGame: () => Promise<Game>
    @SquadModuleAction(GUESS_GAME_RESULT) guessGameResult: (payload: { guess: CrudResultGuess }) => Promise<void>

    creatingGuess: boolean = false
    upcomingGame: Game = new Game()
    selectedNumberOfDifGoals: number = 0
    selectedNumberOfOpponentGoals: number = 0

    mounted() {
        this.loadUpcomingGame()
            .then((game: Game) => {
                this.upcomingGame = game
                if (this.loggedInUsersGameResultGuess) {
                    this.selectedNumberOfDifGoals = this.loggedInUsersGameResultGuess.difGoals
                    this.selectedNumberOfOpponentGoals = this.loggedInUsersGameResultGuess.opponentGoals
                }
            })
        this.setShowLeftSideBar({ value: !this.vm.loggedInUser.filter.hideLeftMenu })
    }

    get user() {
        return this.vm.loggedInUser
    }
    get goals() {
        return [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
    }
    get loggedInUsersGameResultGuess() {
        var gameGuess = this.vm.loggedInUser.gameResultGuesses.filter((guess: GameResultGuess) => guess.game.id == this.upcomingGame.id)[0]
        if (gameGuess) {
            return {
                difGoals: gameGuess.numberOfGoalsScoredByDif,
                opponentGoals: gameGuess.numberOfGoalsScoredByOpponent
            }
        }
        return undefined
    }

    createGuessGameResult() {
        this.creatingGuess = true
        this.guessGameResult({ guess: {
                gameId: this.upcomingGame.id,
                numberOfGoalsScoredByDif: this.selectedNumberOfDifGoals,
                numberOfGoalsScoredByOpponent: this.selectedNumberOfOpponentGoals,
                guessedByUserId: this.vm.loggedInUser.id
            }
        }).then(() => this.creatingGuess = false)
    }
}
</script>
