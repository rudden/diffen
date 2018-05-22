    <template>
    <div class="row" v-if="!saving">
        <div class="col p-0">
            <div class="card card-profile br br__none" style="margin-top: -1rem; margin-bottom: -1rem" v-if="!inEdit">
                <div class="card-header" style="background-image: url(/bg.jpg);"></div>
                <div class="card-body text-center">
                    <img class="card-profile-img" :src="testimonial">
                    <h6 class="card-title">
                        <span class="text-inherit">
                            {{ player.fullName }}
                            <template v-if="hasAnyAttribute">
                                <span class="badge badge-primary ml-1" v-if="player.isCaptain">kapten</span>
                                <span class="badge badge-info ml-1" v-if="player.isViceCaptain">vice kapten</span>
                                <span class="badge badge-danger ml-1" v-if="player.isHereOnLoan">inlånad</span>
                                <span class="badge badge-warning ml-1" v-if="player.isOutOnLoan">utlånad</span>
                                <span class="badge badge-danger ml-1" v-if="player.isSold">såld</span>
                            </template>
                        </span>
                    </h6>
                    <p class="mb-4" v-if="player.about">{{ player.about }}</p>
                    <ul class="card-menu">
                        <li class="card-menu-item" v-if="age > 0">
                            Ålder
                            <h6 class="my-0">{{ age }}</h6>
                        </li>
                        <li class="card-menu-item" v-if="player.heightInCentimeters > 0">
                            Längd
                            <h6 class="my-0">{{ player.heightInCentimeters }}</h6>
                        </li>
                        <li class="card-menu-item" v-if="player.weight > 0">
                            Vikt
                            <h6 class="my-0">{{ player.weight }}</h6>
                        </li>
                        <li class="card-menu-item" v-if="preferredFoot">
                            Fot
                            <h6 class="my-0">{{ preferredFoot }}</h6>
                        </li>
                        <li class="card-menu-item" v-if="player.kitNumber > 0">
                            Nr.
                            <h6 class="my-0">{{ player.kitNumber }}</h6>
                        </li>
                        <li class="card-menu-item" v-if="player.contractUntil">
                            Kontrakt till
                            <h6 class="my-0">{{ player.contractUntil }}</h6>
                        </li>
                    </ul>
                </div>
                <div class="card-body pt-0" v-if="player.events.length > 0">
                    <hr />
                    <strong>Matchstatistik</strong>
                    <hr/>
                    <chartjs-bar :beginzero="true" :labels="chartLabels" :datasets="chartData" />
                </div>
                <div class="card-footer" v-if="loggedInUserIsAdmin">
                    <span class="icon icon-edit float-right" style="cursor: pointer" @click="inEdit = true" v-tooltip="'Editera spelare'"></span>
                </div>
            </div>
            <div v-else>
                <div class="card br br__none" style="margin-top: -1rem; margin-bottom: -1rem">
                    <div class="card-body">
                        <div class="form-group row">
                            <label for="firstName" class="col-sm-3 col-form-label"><strong>Förnamn</strong></label>
                            <div class="col-sm-9">
                                <input id="firstName" type="text" class="form-control form-control-sm" v-model="crudPlayer.firstName" placeholder="Förnamn" required />
                            </div>
                        </div>
                        <div class="form-group row">
                            <label for="lastName" class="col-sm-3 col-form-label"><strong>Efternamn</strong></label>
                            <div class="col-sm-9">
                                <input id="lastName" type="text" class="form-control form-control-sm" v-model="crudPlayer.lastName" placeholder="Efternamn" required />
                            </div>
                        </div>
                        <div class="form-group row">
                            <label for="kitNumber" class="col-sm-3 col-form-label"><strong>Nr.</strong></label>
                            <div class="col-sm-9">
                                <input id="kitNumber" type="number" class="form-control form-control-sm" v-model="crudPlayer.kitNumber" placeholder="Tröjnummer" required />
                            </div>
                        </div>
                        <div class="form-group row">
                            <label for="heightInCm" class="col-sm-3 col-form-label"><strong>Längd (cm)</strong></label>
                            <div class="col-sm-9">
                                <input id="heightInCm" type="number" class="form-control form-control-sm" v-model="crudPlayer.heightInCentimeters" placeholder="Längd i cm" required />
                            </div>
                        </div>
                         <div class="form-group row">
                            <label for="weight" class="col-sm-3 col-form-label"><strong>Vikt</strong></label>
                            <div class="col-sm-9">
                                <input id="weight" type="number" class="form-control form-control-sm" v-model="crudPlayer.weight" placeholder="Vikt" required />
                            </div>
                        </div>
                        <div class="form-group row">
                            <label for="birthDay" class="col-sm-3 col-form-label"><strong>Född</strong></label>
                            <div class="col-sm-9">
                                <v-datepicker v-model="crudPlayer.birthDay" :format="'yyyy-MM-dd'" :bootstrap-styling="true" :input-class="'form-control-sm'" :monday-first="true" :placeholder="'Välj datum'" />
                            </div>
                        </div>
                        <div class="form-group row">
                            <label for="contractUntil" class="col-sm-3 col-form-label"><strong>Kontrakt till</strong></label>
                            <div class="col-sm-9">
                                <v-datepicker v-model="crudPlayer.contractUntil" :format="'yyyy-MM-dd'" :bootstrap-styling="true" :input-class="'form-control-sm'" :monday-first="true" :placeholder="'Välj datum'" />
                            </div>
                        </div>
                        <div class="form-group row">
                            <label for="about" class="col-sm-3 col-form-label"><strong>Om</strong></label>
                            <div class="col-sm-9">
                                <textarea name="about" v-model="crudPlayer.about" rows="5" class="form-control" placeholder="Om spelaren"></textarea>
                            </div>
                        </div>
                        <div class="form-group row">
                            <label for="imageUrl" class="col-sm-3 col-form-label"><strong>Bild</strong></label>
                            <div class="col-sm-9">
                                <input id="imageUrl" type="text" class="form-control form-control-sm" v-model="crudPlayer.imageUrl" placeholder="Länk till bild på spelaren" required />
                            </div>
                        </div>
                        <fieldset class="form-group">
                            <div class="row">
                                <legend class="col-sm-3 col-form-label pt-0"><strong>Attribut</strong></legend>
                                <div class="col-sm-9">
                                    <div class="form-check form-check-inline">
                                        <input type="checkbox" class="form-check-input" id="isCaptain" v-model="crudPlayer.isCaptain" />
                                        <label class="form-check-label" for="isCaptain">Kapten</label>
                                    </div>
                                    <div class="form-check form-check-inline">
                                        <input type="checkbox" class="form-check-input" id="isViceCaptain" v-model="crudPlayer.isViceCaptain" />
                                        <label class="form-check-label" for="isViceCaptain">Vice kapten</label>
                                    </div>
                                    <div class="form-check form-check-inline">
                                        <input type="checkbox" class="form-check-input" id="isOutOnLoan" v-model="crudPlayer.isOutOnLoan" />
                                        <label class="form-check-label" for="isOutOnLoan">Utlånad</label>
                                    </div>
                                    <div class="form-check form-check-inline">
                                        <input type="checkbox" class="form-check-input" id="isHereOnLoan" v-model="crudPlayer.isHereOnLoan" />
                                        <label class="form-check-label" for="isHereOnLoan">Inlånad</label>
                                    </div>
                                    <div class="form-check form-check-inline">
                                        <input type="checkbox" class="form-check-input" id="isSold" v-model="crudPlayer.isSold" />
                                        <label class="form-check-label" for="isSold">Såld</label>
                                    </div>
                                </div>
                            </div>
                        </fieldset>
                        <fieldset class="form-group">
                            <div class="row">
                                <legend class="col-sm-3 col-form-label pt-0"><strong>Fot</strong></legend>
                                <div class="col-sm-9">
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" id="no-foot" v-model="selectedPreferredFoot" value="None" />
                                        <label class="form-check-label" for="no-foot">Ingen</label>
                                    </div>
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" id="left-foot" v-model="selectedPreferredFoot" value="Left" />
                                        <label class="form-check-label" for="left-foot">Vänster</label>
                                    </div>
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" id="right-foot" v-model="selectedPreferredFoot" value="Right" />
                                        <label class="form-check-label" for="right-foot">Höger</label>
                                    </div>
                                </div>
                            </div>
                        </fieldset>
                        <fieldset class="form-group">
                            <div class="row">
                                <legend class="col-sm-3 col-form-label pt-0"><strong>Positioner</strong></legend>
                                <div class="col-sm-9">
                                    <div class="form-check form-check-inline" v-for="position in positions" v-bind:key="position.id">
                                        <input class="form-check-input" type="checkbox" :id="position.id" :value="position.id" v-model="crudPlayer.availablePositionsIds">
                                        <label class="form-check-label" :for="position.id">{{ position.name }}</label>
                                    </div>
                                </div>
                            </div>
                        </fieldset>
                        <div class="alert alert-danger" v-if="takenKitNumber">Tröjnumret är upptaget</div>
                        <results :items="results" class="mb-3" />
                    </div>
                    <div class="card-footer">
                        <div class="row">
                            <div class="col pr-1">
                                <button class="btn btn-success btn-block btn-sm" :disabled="!canSave" v-on:click="onSave">Spara</button>
                            </div>
                            <div class="col pl-1">
                                <button class="btn btn-danger btn-block btn-sm" v-on:click="inEdit = false">Avbryt</button>
                            </div>
                        </div>
                    </div>
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
import { Player, Position, PlayerEventOnPlayer, GameType, GameEventType, PlayerEvent, PreferredFoot } from '../../../model/squad'
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

    selectedPreferredFoot: string = 'None'

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

    get age() {
        return this.player.birthDay ? new Date().getFullYear() - new Date(this.player.birthDay).getFullYear() : 0
    }
    get testimonial() {
        return this.player.imageUrl ? this.player.imageUrl : '/gorilla.jpg'
    }
    get preferredFoot() {
        switch (this.player.preferredFoot) {
            case PreferredFoot.None:
                return ''
            case PreferredFoot.Left:
                return 'Vänster'
            case PreferredFoot.Right:
                return 'Höger'
        }
    }
    get hasAnyAttribute() {
        return this.player && (this.player.isCaptain || this.player.isViceCaptain || this.player.isSold || this.player.isOutOnLoan || this.player.isHereOnLoan)
    }

    get loggedInUserIsAdmin(): boolean {
        return this.vm.loggedInUser.inRoles.some(role => role == 'Admin' || role == 'GameAdmin')
    }
    
    chartColors(color: string) {
        let colors: string[] = []
        for (let i = 0; i < this.chartLabels.length; i++) {
            colors.push(color)
        }
        return colors
    }
    get chartLabels() {
        return ['Mål', 'Assist', 'Gula kort', 'Röda kort', 'Inbytt', 'Utbytt']
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
            this.getNumberOfEvents(gameType, GameEventType.RedCard),
            this.getNumberOfEvents(gameType, GameEventType.SubstituteIn),
            this.getNumberOfEvents(gameType, GameEventType.SubstituteOut)
        ]
    }
    getNumberOfEvents(gameType: GameType, eventType: GameEventType) {
        return this.player.events.filter((e: PlayerEventOnPlayer) => e.gameType == gameType && e.eventType == eventType).length
    }
    
    onSave() {
        this.saving = true
        this.crudPlayer.preferredFoot= PreferredFoot[this.selectedPreferredFoot as keyof typeof PreferredFoot]
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
            this.selectedPreferredFoot = PreferredFoot[player.preferredFoot]
			this.crudPlayer = <CrudPlayer> {
				id: player.id,
				firstName: player.firstName,
				lastName: player.lastName,
				kitNumber: player.kitNumber,
				isSold: player.isSold,
                isCaptain: player.isCaptain,
				isViceCaptain: player.isViceCaptain,
				isHereOnLoan: player.isHereOnLoan,
                isOutOnLoan: player.isOutOnLoan,
                birthDay: player.birthDay ? new Date(player.birthDay) : undefined,
                heightInCentimeters: player.heightInCentimeters,
                weight: player.weight,
                preferredFoot: player.preferredFoot,
                about: player.about,
                contractUntil: player.contractUntil ? new Date(player.contractUntil) : undefined,
                imageUrl: player.imageUrl,
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