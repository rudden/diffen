    <template>
    <div class="row" v-if="!saving">
        <div class="col">
            <div class="form-group row">
                <label for="firstName" class="col-sm-2 col-form-label">Förnamn</label>
                <div class="col-sm-10">
                    <input id="firstName" type="text" :class="{ 'form-control': inEdit, 'form-control-plaintext': !inEdit }" v-model="crudPlayer.firstName" placeholder="Förnamn" required :readonly="!inEdit" />
                </div>
            </div>
            <div class="form-group row">
                <label for="lastName" class="col-sm-2 col-form-label">Efternamn</label>
                <div class="col-sm-10">
                    <input id="lastName" type="text" :class="{ 'form-control': inEdit, 'form-control-plaintext': !inEdit }" v-model="crudPlayer.lastName" placeholder="Efternamn" required :readonly="!inEdit" />
                </div>
            </div>
            <div class="form-group row" v-if="inEdit || player && player.kitNumber > 0">
                <label for="kitNumber" class="col-sm-2 col-form-label">Tröjnummer</label>
                <div class="col-sm-10">
                    <input id="kitNumber" type="number" :class="{ 'form-control': inEdit, 'form-control-plaintext': !inEdit }" v-model="crudPlayer.kitNumber" placeholder="Tröjnummer" required :readonly="!inEdit" />
                </div>
            </div>
            <fieldset class="form-group" v-if="inEdit || hasAnyAttribute">
                <div class="row">
                    <legend class="col-sm-2 col-form-label pt-0">Attribut</legend>
                    <div class="col-sm-10">
                        <template v-if="inEdit">
                            <div class="form-check">
                                <input type="checkbox" class="form-check-input" id="isCaptain" v-model="crudPlayer.isCaptain" />
                                <label class="form-check-label" for="isCaptain">Kapten</label>
                            </div>
                            <div class="form-check">
                                <input type="checkbox" class="form-check-input" id="isViceCaptain" v-model="crudPlayer.isViceCaptain" />
                                <label class="form-check-label" for="isViceCaptain">Vice kapten</label>
                            </div>
                            <div class="form-check">
                                <input type="checkbox" class="form-check-input" id="isOutOnLoan" v-model="crudPlayer.isOutOnLoan" />
                                <label class="form-check-label" for="isOutOnLoan">Utlånad</label>
                            </div>
                            <div class="form-check">
                                <input type="checkbox" class="form-check-input" id="isHereOnLoan" v-model="crudPlayer.isHereOnLoan" />
                                <label class="form-check-label" for="isHereOnLoan">Inlånad</label>
                            </div>
                            <div class="form-check">
                                <input type="checkbox" class="form-check-input" id="isSold" v-model="crudPlayer.isSold" />
                                <label class="form-check-label" for="isSold">Såld</label>
                            </div>
                        </template>
                        <template v-else>
                            <span class="badge badge-primary ml-1" v-if="player.isCaptain">kapten</span>
                            <span class="badge badge-info ml-1" v-if="player.isViceCaptain">vice kapten</span>
                            <span class="badge badge-danger ml-1" v-if="player.isHereOnLoan">inlånad</span>
                            <span class="badge badge-warning ml-1" v-if="player.isOutOnLoan">utlånad</span>
                            <span class="badge badge-danger ml-1" v-if="player.isSold">såld</span>
                        </template>
                    </div>
                </div>
            </fieldset>
            <fieldset class="form-group">
                <div class="row">
                    <legend class="col-sm-2 col-form-label pt-0">Positioner</legend>
                    <div class="col-sm-10">
                        <template v-if="inEdit">
                            <div class="form-check form-check-inline" v-for="position in positions" v-bind:key="position.id">
                                <input class="form-check-input" type="checkbox" :id="position.id" :value="position.id" v-model="crudPlayer.availablePositionsIds">
                                <label class="form-check-label" :for="position.id">{{ position.name }}</label>
                            </div>
                        </template>
                        <template v-else-if="!inEdit && player">
                            <span class="badge badge-dark ml-1" v-for="position in player.availablePositions" :key="position.id">{{ position.name }}</span>
                        </template>
                    </div>
                </div>
            </fieldset>
            <div class="form-group" v-if="!inEdit && player && player.events.length > 0">
                <strong>Statistik</strong>
                <hr/>
                <chartjs-bar :beginzero="true" :labels="chartLabels" :datasets="chartData" />
            </div>
            <div class="form-group row mb-0" v-if="loggedInUserIsAdmin && player">
                <div class="col">
                    <span class="icon icon-edit float-right" @click="inEdit = !inEdit" style="cursor: pointer"></span>
                </div>
            </div>
            <div class="alert alert-danger" v-if="takenKitNumber">Tröjnumret är upptaget</div>
            <results :items="results" class="mb-3" />
            <div class="row" v-if="inEdit">
                <div class="col">
                    <button class="btn btn-success btn-block btn-sm" :disabled="!canSave" v-on:click="onSave">Spara</button>
                </div>
            </div>
        </div>
    </div>
    <div v-else>
        <loader v-bind="{ background: '#699ED0' }" />
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Action, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('squad', Getter)
const ModuleAction = namespace('squad', Action)

import { PageViewModel, Result, ResultType } from '../../../model/common'
import { Player, Position, PlayerEventOnPlayer, GameType, GameEventType } from '../../../model/squad'
import { Player as CrudPlayer } from '../../../model/squad/crud'

import {
    GET_PLAYERS,
    GET_POSITIONS,
    FETCH_PLAYERS
} from '../../../modules/squad/types'

import Modal from '../../../components/modal.vue'
import Results from '../../../components/results.vue'

@Component({
    props: {
        player: Object,
        save: Function
    },
	components: {
		Modal, Results
	}
})
export default class PlayerComponent extends Vue {
    @State(state => state.vm) vm: PageViewModel
	@ModuleGetter(GET_PLAYERS) players: Player[]
	@ModuleGetter(GET_POSITIONS) positions: Position[]
	@ModuleAction(FETCH_PLAYERS) loadPlayers: () => Promise<void>
    
    player: Player
    save: (player: CrudPlayer) => Promise<Result[]>

	crudPlayer: CrudPlayer = new CrudPlayer()

    saving: boolean = false
    inEdit: boolean = false
    results: Result[] = []

    private yellow: string = '#E0A541'
    private red: string = '#E21F26'
    private darkBlue: string = '#0568AF'
    private lightBlue: string = '#699ED0'
    
    mounted() {
        if (!this.player) {
            this.inEdit = true
        } else {
            this.setPlayer(this.player)
        }
    }

	get takenKitNumber() {
        if (this.crudPlayer.kitNumber == 0)
            return false
		let player = this.players.filter((p: Player) => p.id == this.crudPlayer.id)[0]
		if (player) {
			if (player.kitNumber == this.crudPlayer.kitNumber)
				return false
		}
        return this.players.map((p: Player) => p.kitNumber).includes(parseInt(this.crudPlayer.kitNumber.toString()))
	}

	get canSave() {
		return this.crudPlayer.firstName.length > 2
			&& this.crudPlayer.lastName.length > 2
			&& this.crudPlayer.kitNumber < 100
			&& !this.takenKitNumber
			&& this.crudPlayer.availablePositionsIds.length > 0
    }

    get hasAnyAttribute() {
        return this.player && (this.player.isCaptain || this.player.isViceCaptain || this.player.isSold || this.player.isOutOnLoan || this.player.isHereOnLoan)
    }

    get loggedInUserIsAdmin(): boolean {
        return this.vm.loggedInUser.inRoles.some(role => role == 'Admin')
    }
    
    chartColors(color: string) {
        let colors: string[] = []
        for (let i = 0; i < this.chartLabels.length; i++) {
            colors.push(color)
        }
        return colors
    }
    get chartLabels() {
        return ['Mål', 'Assist', 'Gula kort', 'Röda kort']
    }
    get chartData() {
        return [
            this.getChartItem('Allsvenskan', GameType.League, this.yellow),
            this.getChartItem('Cupen', GameType.Cup, this.red),
            this.getChartItem('Europa League', GameType.EuropaLeague, this.darkBlue),
            this.getChartItem('Träningsmatch', GameType.Training, this.lightBlue)
        ]
    }

    getChartItem(label: string, gameType: GameType, color: string) {
        return {
            label: label,
            data: this.getPlayerEventData(gameType),
            borderColor: this.chartColors(color), backgroundColor: this.chartColors(color)
        }
    }
    getPlayerEventData(gameType: GameType) {
        return [
            this.getNumberOfEvents(gameType, GameEventType.Goal),
            this.getNumberOfEvents(gameType, GameEventType.Assist),
            this.getNumberOfEvents(gameType, GameEventType.YellowCard),
            this.getNumberOfEvents(gameType, GameEventType.RedCard)
        ]
    }
    getNumberOfEvents(gameType: GameType, eventType: GameEventType) {
        return this.player.events.filter((e: PlayerEventOnPlayer) => e.gameType == gameType && e.eventType == eventType).length
    }
    
    onSave() {
        this.saving = true
        this.save(this.crudPlayer)
            .then((results: Result[]) => {
                    this.loadPlayers()
                        .then(() => {
                            this.setPlayer(this.players.filter((p: Player) => p.firstName == this.crudPlayer.firstName && p.lastName == this.crudPlayer.lastName)[0])
                            this.saving = false
                        })
                    this.results = results
                })
    }

    setPlayer(player: Player) {
        if (!player) {
			this.crudPlayer = new CrudPlayer()
		} else {
			this.crudPlayer = {
				id: player.id,
				firstName: player.firstName,
				lastName: player.lastName,
				kitNumber: player.kitNumber,
				isSold: player.isSold,
                isCaptain: player.isCaptain,
				isViceCaptain: player.isViceCaptain,
				isHereOnLoan: player.isHereOnLoan,
				isOutOnLoan: player.isOutOnLoan,
				availablePositionsIds: player.availablePositions.map((pos: Position) => pos.id)
			}
		}
    }
}
</script>

<style lang="scss" scoped>
a {
	cursor: pointer;
}
</style>