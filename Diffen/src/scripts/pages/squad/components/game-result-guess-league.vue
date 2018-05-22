<template>
	<div>
        <ul class="list-group media-list media-list-stream">
            <li class="list-group-item p-4">
                <modal v-bind="modalAttributes.new">
                    <template slot="body">
                        <game-guesser :guesser-only="true" />
                    </template>
                </modal>
                <h4 class="mb-0">Tipsligan</h4>
            </li>
            <li class="list-group-item media">
                <template v-if="!loading">
                    <table-component :data="table" sort-by="id" sort-order="desc" :show-filter="false" @rowClick="rowClick">
                        <table-column label="#" show="id" data-type="numeric"></table-column>
                        <table-column label="Användare" show="user.nickName" :sortable="false"></table-column>
                        <table-column label="Poäng" show="points" data-type="numeric"></table-column>
                        <table-column label="Antal tippningar" show="guesses.length" data-type="numeric"></table-column>
                        <template slot="tfoot">
                            <tr>
                                <td colspan="4" style="border-top: 0">
                                    <hr />
                                    <modal v-bind="modalAttributes.info">
                                        <template slot="body">
                                            <ul class="list-unstyled list-spaced mb-0">
                                                <li><strong>1 poäng</strong> - Rätt antal mål för DIF eller motståndare</li>
                                                <li><strong>2 poäng</strong> - Rätt antal totala mål</li>
                                                <li><strong>3 poäng</strong> - Rätt resultat</li>
                                            </ul>
                                        </template>
                                    </modal>
                                </td>
                            </tr>
                        </template>
                    </table-component>
                    <modal v-for="item in table" :key="item.id" v-bind="{ attributes: { name: `show-data-${item.user.id}`, scrollable: true }, header: item.user.nickName, button: { } }">
                        <template slot="body">
                            <div class="row">
                                <div class="col">
                                    <table-component :data="userTable" sort-by="playedDate" sort-order="desc" :show-filter="false" :table-class="'table table-sm mb-0'">
                                        <table-column label="Spelades" show="playedDate"></table-column>
                                        <table-column label="Hemmalag" :sortable="false">
                                            <template slot-scope="row">
                                                <strong>{{ row.homeTeam.name }}</strong>
                                                <br />
                                                Gjorda mål: {{ row.homeTeam.goal.outcome }}
                                                <br />
                                                Gissade: {{ row.homeTeam.goal.guess }}
                                            </template>
                                        </table-column>
                                        <table-column label="Bortalag" :sortable="false">
                                            <template slot-scope="row">
                                                <strong>{{ row.awayTeam.name }}</strong>
                                                <br />
                                                Gjorda mål: {{ row.awayTeam.goal.outcome }}
                                                <br />
                                                Gissade: {{ row.awayTeam.goal.guess }}
                                            </template>
                                        </table-column>
                                        <table-column label="Poäng" show="points" data-type="numeric"></table-column>
                                    </table-component>
                                </div>
                            </div>
                        </template>
                    </modal>
                </template>
                <template v-else>
                    <loader v-bind="{ background: '#699ED0' }" />
                </template>
            </li>
        </ul>
	</div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component, Watch } from 'vue-property-decorator'
import { Getter, Action, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('squad', Getter)
const ModuleAction = namespace('squad', Action)

import { Game, GameResultGuess, PlayerEvent, GameEventType, GameResultGuessLeagueItem, ArenaType } from '../../../model/squad'
import { PageViewModel, Result, ResultType, IdAndNickNameUser } from '../../../model/common'

import { FETCH_FINISHED_GAME_RESULT_GUESSES } from '../../../modules/squad/types'

interface IGuessLeagueItem {
    id?: number
    user: IdAndNickNameUser
    result: IGuessResult
    guesses: GameResultGuess[]
    points?: number
}

interface IGoal {
    outcome: number
    guess: number
}

interface ITeam {
    name: string
    goal: IGoal
}

interface IGuess {
    homeTeam: ITeam
    awayTeam: ITeam
    playedDate: string
    points: number
}

interface IGuessResult {
    numberOfCorrectDifGoalGuesses: number
    numberOfCorrectOpponentGoalGuesses: number
    numberOfCorrectResultGuesses: number
    numberOfCorrectAmountOfGoalsGuesses: number
}

import Modal from '../../../components/modal.vue'
import GameGuesser from '../../../components/game-guesser.vue'

@Component({
	components: {
		Modal, GameGuesser
	}
})
export default class GameGuessResultLeage extends Vue {
	@State(state => state.vm) vm: PageViewModel
    @ModuleAction(FETCH_FINISHED_GAME_RESULT_GUESSES) loadFinishedGameResultGuesses: () => Promise<GameResultGuessLeagueItem[]>

    loading: boolean = true
    guesses: GameResultGuessLeagueItem[] = []

    modalAttributes: any = {
		info: {
			attributes: {
				name: 'info',
				scrollable: true
			},
			header: 'Hur poängen räknas ut',
			button: {
				icon: 'icon icon-info float-right',
				text: 'Visa hur poängen räknas ut'
            }
        },
        new: {
			attributes: {
				name: 'new',
				scrollable: true
			},
			header: 'Hur slutar nästa match?',
			button: {
				icon: 'icon icon-plus float-right',
				text: 'Tippa nästa match'
            }
		}
    }

    selectedUserId: string = ''

	$modal: any = (this as any).VModal

	mounted() {
        this.loadFinishedGameResultGuesses()
            .then((guesses: GameResultGuessLeagueItem[]) => {
                this.guesses = guesses
                this.loading = false
            })
    }
    
    get table() {
        return this.complementTableItems(
            this.guesses.map((g: GameResultGuessLeagueItem) => {
                return <IGuessLeagueItem> {
                    user: g.user,
                    result: this.getNumberOfCorrectResultGuesses(g.guesses),
                    guesses: g.guesses
                }
            })
        )
    }

    get userTable() {
        var item = this.guesses.filter((g: GameResultGuessLeagueItem) => g.user.id == this.selectedUserId)[0]
        if (item) {
            return item.guesses.map((guess: GameResultGuess) => {
                    return <IGuess> {
                        homeTeam: this.getTeamData(guess.game.arenaType == ArenaType.Home, guess),
                        awayTeam: this.getTeamData(guess.game.arenaType == ArenaType.Away, guess),
                        playedDate: guess.game.playedOn,
                        points: this.getPoints(this.getPointForGuess(guess))
                    }
                })
        }
    }

    getTeamData(difIsHomeTeam: boolean, guess: GameResultGuess): ITeam {
        return {
            name: difIsHomeTeam ? 'DIF' : guess.game.opponent,
            goal: {
                outcome: difIsHomeTeam ? guess.game.playerEvents.filter((e: PlayerEvent) => e.eventType == GameEventType.Goal).length : guess.game.numberOfGoalsScoredByOpponent,
                guess: difIsHomeTeam ? guess.numberOfGoalsScoredByDif : guess.numberOfGoalsScoredByOpponent
            }
        }
    }

    complementTableItems(items: IGuessLeagueItem[]) {
        items.map((item: IGuessLeagueItem) => {
            item.points = this.getPoints(item.result)
            return item
        })
        items.sort((a: IGuessLeagueItem, b: IGuessLeagueItem) => {
            return <number>b.points - <number>a.points
        })
        items.map((item: IGuessLeagueItem, index) => {
            item.id = index + 1
            return item
        })
        return items
    }

    getNumberOfCorrectResultGuesses(guesses: GameResultGuess[]): IGuessResult {
        let guessResults: IGuessResult[] = []
        guesses.forEach((guess: GameResultGuess) => guessResults.push(this.getPointForGuess(guess)))
        return <IGuessResult> {
            numberOfCorrectResultGuesses: guessResults.map((result: IGuessResult) => result.numberOfCorrectResultGuesses).reduce((acc: number, val: number) => { return acc + val }),
            numberOfCorrectDifGoalGuesses: guessResults.map((result: IGuessResult) => result.numberOfCorrectDifGoalGuesses).reduce((acc: number, val: number) => { return acc + val }),
            numberOfCorrectOpponentGoalGuesses: guessResults.map((result: IGuessResult) => result.numberOfCorrectOpponentGoalGuesses).reduce((acc: number, val: number) => { return acc + val }),
            numberOfCorrectAmountOfGoalsGuesses: guessResults.map((result: IGuessResult) => result.numberOfCorrectAmountOfGoalsGuesses).reduce((acc: number, val: number) => { return acc + val }),
        }
    }

    getTotalPoints(guessResults: IGuessResult[]) {
        return 
    }

    getPointForGuess(guess: GameResultGuess) {
        let numberOfCorrectDifGoalGuesses: number = 0
        let numberOfCorrectOpponentGoalGuesses: number = 0
        let numberOfCorrectResultGuesses: number = 0
        let numberOfCorrectAmountOfGoalsGuesses: number = 0

        let numberOfGoalsScoredByOpponent: number = guess.game.numberOfGoalsScoredByOpponent
        let guessForNumberOfGoalsScoredByOpponent: number = guess.numberOfGoalsScoredByOpponent

        let numberOfGoalsScoredByDif: number = guess.game.playerEvents.filter((e: PlayerEvent) => e.eventType == GameEventType.Goal).length
        let guessForNumberOfGoalsScoredByDif: number = guess.numberOfGoalsScoredByDif

        if (
            numberOfGoalsScoredByDif == guessForNumberOfGoalsScoredByDif &&
            numberOfGoalsScoredByOpponent == guessForNumberOfGoalsScoredByOpponent
        ) {
            numberOfCorrectResultGuesses++
        } else {
            if (numberOfGoalsScoredByDif == guessForNumberOfGoalsScoredByDif)
                numberOfCorrectDifGoalGuesses++
        
            if (numberOfGoalsScoredByOpponent == guessForNumberOfGoalsScoredByOpponent)
                numberOfCorrectOpponentGoalGuesses++

            let amountOfGoalsScoredGuess: number = guessForNumberOfGoalsScoredByDif + guessForNumberOfGoalsScoredByOpponent
            let amountOfGoalsScoredOutcome: number = numberOfGoalsScoredByDif + numberOfGoalsScoredByOpponent
            if (amountOfGoalsScoredGuess == amountOfGoalsScoredOutcome)
                numberOfCorrectAmountOfGoalsGuesses++
        }
        return <IGuessResult> {
            numberOfCorrectResultGuesses: numberOfCorrectResultGuesses,
            numberOfCorrectDifGoalGuesses: numberOfCorrectDifGoalGuesses,
            numberOfCorrectOpponentGoalGuesses: numberOfCorrectOpponentGoalGuesses,
            numberOfCorrectAmountOfGoalsGuesses: numberOfCorrectAmountOfGoalsGuesses
        }
    }

    getPoints(result: IGuessResult): number {
        var points: number = 0
        points += result.numberOfCorrectDifGoalGuesses
        points += result.numberOfCorrectOpponentGoalGuesses
        points += (result.numberOfCorrectAmountOfGoalsGuesses) * 2
        points += (result.numberOfCorrectResultGuesses) * 3
        return points
    }

    rowClick(row: any) {
        this.selectedUserId = row.data.user.id
        this.$modal.show(`show-data-${this.selectedUserId}`)
    }
}
</script>