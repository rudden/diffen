<template>
	<div class="row" style="margin: -1rem">
        <div class="col p-0">
            <ul class="list-group media-list media-list-stream">
                <li class="list-group-item p-4">
                    <button class="btn btn-sm btn-primary float-right" @click="showRules = !showRules">{{ showRules ? 'Dölj' : 'Visa' }} regler</button>
                    <h4 class="mb-0">Tipsligan</h4>
                </li>
                <li class="list-group-item media p-4" v-if="showRules">
                    <div class="alert alert-info mb-0">
                        <ul class="list-unstyled list-spaced">
                            <li><strong>4 poäng</strong> - Rätt resultat</li>
                            <li><strong>2 poäng</strong> - Rätt tippning (1X2)</li>
                            <li><strong>1 poäng</strong> - Rätt antal DIF mål (endast chans på detta om man inte redan prickat helt rätt resultat)</li>
                            <li><strong>1 poäng</strong> - Rätt antal moståndarmål (endast chans på detta om man inte redan prickat helt rätt resultat)</li>
                            <li><strong>1 poäng</strong> - Rätt antal totala mål i matchen (endast chans på detta om man inte redan prickat helt rätt resultat)</li>
                        </ul>
                    </div>
                </li>
                <li class="list-group-item media p-4">
                    <template v-if="!loading">
                        <table-component :data="table" sort-by="id" sort-order="asc" :show-filter="false" @rowClick="rowClick" v-if="selectedUserId == 0">
                            <table-column label="#" show="id" data-type="numeric"></table-column>
                            <table-column label="Användare" show="user.nickName" :sortable="false"></table-column>
                            <table-column label="Poäng" show="points" data-type="numeric"></table-column>
                            <table-column label="Tippat ggr" show="guesses.length" data-type="numeric"></table-column>
                        </table-component>
                        <template v-else>
                            <div class="col pl-0 pr-0">
                                <div class="flow-root">
                                    <span class="float-right" @click="selectedUserId = 0" style="cursor: pointer; font-weight: bold; margin-bottom: -1rem" v-tooltip="'Visa tabellen'">&times;</span>
                                    <h6 class="float-left mb-0">{{ userTable.user.nickName }} tippningar</h6>
                                </div>
                                <hr />
                                <ul class="list-group mb-0">
                                    <li class="list-group-item" v-for="guess in userTable.guesses" :key="guess.playedDate">
                                        <div class="flow-root">
                                            <small class="float-right text-muted">
                                                <span class="icon icon-clock"></span>
                                                {{ guess.playedDate }}
                                            </small>
                                            <h6 class="float-left">{{ guess.homeTeam.name }} - {{ guess.awayTeam.name }}</h6>
                                        </div>
                                        <strong>Utfall:</strong> {{ guess.homeTeam.goal.outcome }} - {{ guess.awayTeam.goal.outcome }} ({{ guess.homeTeam.goal.guess }} - {{ guess.awayTeam.goal.guess }})
                                        <span class="badge badge-pill float-right" :class="{ 'badge-success': guess.points >= 2, 'badge-primary': guess.points == 1, 'badge-danger': guess.points == 0 }">{{ guess.points }}p</span>
                                    </li>
                                    <li class="list-group-item">
                                        <span class="badge badge-pill float-right" :class="{ 'badge-success': userTable.points > 0, 'badge-danger': userTable.points == 0 }">
                                            {{ userTable.points }}p
                                        </span>
                                        <strong>Totalt</strong>
                                    </li>
                                </ul>
                            </div>
                        </template>
                    </template>
                    <template v-else>
                        <loader v-bind="{ background: '#699ED0' }" />
                    </template>
                </li>
            </ul>
        </div>
	</div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component, Watch } from 'vue-property-decorator'
import { Getter, Action, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('squad', Getter)
const ModuleAction = namespace('squad', Action)

import { Game, GameResultGuess, PlayerEvent, GameEventType, GameResultGuessLeagueItem, ArenaType } from '../model/squad'
import { PageViewModel, Result, ResultType, IdAndNickNameUser } from '../model/common'

import { FETCH_FINISHED_GAME_RESULT_GUESSES } from '../modules/squad/types'

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

interface IUserGuesses {
    user: IdAndNickNameUser
    guesses: IGuess[]
    points: number
}

interface IGuessResult {
    numberOfCorrectDifGoalGuesses: number
    numberOfCorrectOpponentGoalGuesses: number
    numberOfCorrectResultGuesses: number
    numberOfCorrectAmountOfGoalsGuesses: number
    numberOfCorrectGameOutcomeGuesses: number
}

@Component({})
export default class GameGuessResultLeage extends Vue {
	@State(state => state.vm) vm: PageViewModel
    @ModuleAction(FETCH_FINISHED_GAME_RESULT_GUESSES) loadFinishedGameResultGuesses: () => Promise<GameResultGuessLeagueItem[]>

    loading: boolean = true
    guesses: GameResultGuessLeagueItem[] = []

    showRules: boolean = false

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
            var table = <IUserGuesses> {
                user: item.user,
                guesses: item.guesses.map((guess: GameResultGuess) => {
                    return <IGuess> {
                        homeTeam: this.getTeamData(guess.game.arenaType == ArenaType.Home, guess),
                        awayTeam: this.getTeamData(guess.game.arenaType == ArenaType.Away, guess),
                        playedDate: guess.game.playedOn,
                        points: this.getPoints(this.getPointForGuess(guess))
                    }
                })
            }
            table.points = table.guesses.map(g => g.points).reduce((acc, val) => { return acc + val })
            return table
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
            numberOfCorrectGameOutcomeGuesses: guessResults.map((result: IGuessResult) => result.numberOfCorrectGameOutcomeGuesses).reduce((acc: number, val: number) => { return acc + val }),
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
        let numberOfCorrectGameOutcomeGuesses: number = 0

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
            if (this.is1x2(guess.game.arenaType, numberOfGoalsScoredByDif, guessForNumberOfGoalsScoredByDif, numberOfGoalsScoredByOpponent, guessForNumberOfGoalsScoredByOpponent)) {
                numberOfCorrectGameOutcomeGuesses++
            }
            if (numberOfGoalsScoredByDif == guessForNumberOfGoalsScoredByDif) {
                numberOfCorrectDifGoalGuesses++
            }
        
            if (numberOfGoalsScoredByOpponent == guessForNumberOfGoalsScoredByOpponent) {
                numberOfCorrectOpponentGoalGuesses++
            }

            let amountOfGoalsScoredGuess: number = guessForNumberOfGoalsScoredByDif + guessForNumberOfGoalsScoredByOpponent
            let amountOfGoalsScoredOutcome: number = numberOfGoalsScoredByDif + numberOfGoalsScoredByOpponent
            if (amountOfGoalsScoredGuess == amountOfGoalsScoredOutcome) {
                numberOfCorrectAmountOfGoalsGuesses++
            }
        }
        return <IGuessResult> {
            numberOfCorrectResultGuesses: numberOfCorrectResultGuesses,
            numberOfCorrectDifGoalGuesses: numberOfCorrectDifGoalGuesses,
            numberOfCorrectOpponentGoalGuesses: numberOfCorrectOpponentGoalGuesses,
            numberOfCorrectAmountOfGoalsGuesses: numberOfCorrectAmountOfGoalsGuesses,
            numberOfCorrectGameOutcomeGuesses: numberOfCorrectGameOutcomeGuesses
        }
    }

    is1x2(arenaType: ArenaType, difGoals: number, difGoalsGuess: number, opponentGoals: number, opponentGoalsGuess: number) {
        if (
            // home win
            (arenaType == ArenaType.Home && difGoalsGuess > opponentGoalsGuess && difGoals > opponentGoals) ||
            // home loss
            (arenaType == ArenaType.Home && difGoalsGuess < opponentGoalsGuess && difGoals < opponentGoals) ||
            // away win
            (arenaType == ArenaType.Away && difGoalsGuess > opponentGoalsGuess && difGoals > opponentGoals) ||
            (arenaType == ArenaType.Away && difGoalsGuess < opponentGoalsGuess && difGoals < opponentGoals) ||
            // tied
            difGoals == opponentGoals
        ) return true
        return false
    }

    getPoints(result: IGuessResult): number {
        var points: number = 0
        points += result.numberOfCorrectDifGoalGuesses
        points += result.numberOfCorrectOpponentGoalGuesses
        points += result.numberOfCorrectAmountOfGoalsGuesses
        points += result.numberOfCorrectGameOutcomeGuesses * 2
        points += result.numberOfCorrectResultGuesses * 4
        return points
    }

    rowClick(row: any) {
        this.selectedUserId = row.data.user.id
    }
}
</script>

<style lang="scss" scoped>
.list-group-item {
    border-radius: 0 !important;
}
</style>
