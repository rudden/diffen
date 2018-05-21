<template>
	<div>
        <ul class="list-group media-list media-list-stream">
            <li class="list-group-item p-4">
                <modal v-bind="modalAttributes.info">
                    <template slot="body">
                        <ul class="list-unstyled list-spaced mb-0">
                            <li><strong>1 poäng</strong> - Rätt antal mål för DIF eller motståndare</li>
                            <li><strong>2 poäng</strong> - Rätt antal totala mål</li>
                            <li><strong>3 poäng</strong> - Rätt resultat</li>
                        </ul>
                    </template>
                </modal>
                <h4 class="mb-0">Tipsligan</h4>
            </li>
            <li class="list-group-item media">
                <template v-if="!loading">
                    <table-component :data="table" sort-by="id" sort-order="desc" :show-filter="false" :table-class="'table table-sm mb-0'">
                        <table-column label="#" show="id" data-type="numeric"></table-column>
                        <table-column label="Användare" show="user.nickName" :sortable="false"></table-column>
                        <table-column label="Poäng" show="points" data-type="numeric"></table-column>
                        <table-column label="Antal tippningar" show="numberOfGuesses" data-type="numeric"></table-column>
                    </table-component>
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

import { Game, GameResultGuess, PlayerEvent, GameEventType, GameResultGuessLeagueItem } from '../../../model/squad'
import { PageViewModel, Result, ResultType, IdAndNickNameUser } from '../../../model/common'

import { FETCH_FINISHED_GAME_RESULT_GUESSES } from '../../../modules/squad/types'

import Modal from '../../../components/modal.vue'

interface IGuessLeagueItem {
    id?: number
    user: IdAndNickNameUser
    result: IGuessResult
    numberOfGuesses: number
    points?: number
}

interface IGuessResult {
    numberOfCorrectDifGoalGuesses: number
    numberOfCorrectOpponentGoalGuesses: number
    numberOfCorrectResultGuesses: number
    numberOfCorrectAmountOfGoalsGuesses: number
}

@Component({
	components: {
		Modal
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
		}
    }

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
                    numberOfGuesses: g.guesses.length
                }
            })
        )
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
        let numberOfCorrectDifGoalGuesses: number = 0
        let numberOfCorrectOpponentGoalGuesses: number = 0
        let numberOfCorrectResultGuesses: number = 0
        let numberOfCorrectAmountOfGoalsGuesses: number = 0

        guesses.forEach((guess: GameResultGuess) => {
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
        })
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
}
</script>