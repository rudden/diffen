<template>
	<div class="card">
        <div class="card-body">
            <template v-if="!loading">
                <div class="row">
                    <div class="col">
                        <span class="text-muted float-right">
                            <span class="badge" :class="{ 'badge-warning': poll.isOpen, 'badge-danger': !poll.isOpen }">
                                {{ poll.isOpen ? `stänger om ${daysUntilPollCloses(poll)} dagar` : 'stängd' }}
                            </span>
                        </span>
                        <h4 class="mb-3">Av: {{ poll.byUser.nickName }}</h4>
                    </div>
                </div>
                <hr />
                <div class="row">
                    <div class="col">
                        <chartjs-horizontal-bar :beginzero="true" :datalabel="poll.name" :labels="labels" :data="values" :bordercolor="'#162248'" :backgroundcolor="'#162248'" :bind="true" />
                    </div>
                </div>
            </template>
            <template v-else>
                <div class="row col">
                    <loader v-bind="{ background: '#699ED0' }" />
                </div>
            </template>
        </div>
        <div class="card-footer" v-if="!loading">
            <div class="row">
                <template v-if="placedVoteId <= 0 && poll.isOpen">
                    <div class="col-8">
                        <select class="form-control form-control-sm" v-model="pollSelection">
                            <option value="0">Välj alternativ</option>
                            <option v-for="selection in poll.selections" :value="selection.id" :key="selection.id">{{ selection.name }}</option>
                        </select>
                    </div>
                    <div class="col-4 mt-0">
                        <button class="btn btn-success btn-block btn-sm" v-on:click="vote" :disabled="pollSelection == 0">Rösta</button>
                    </div>
                </template>
                <div class="col" v-if="placedVoteId > 0">
                    <div class="alert alert-info mb-0">Du röstade: <strong>{{ selectedVote.name }}</strong></div>
                </div>
            </div>
        </div>
	</div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Action, State, namespace } from 'vuex-class'

import moment from 'moment'

const ModuleGetter = namespace('other', Getter)
const ModuleAction = namespace('other', Action)

import { GET_POLL, FETCH_POLL, CREATE_POLL_VOTE } from '../../modules/other/types'

import { PollViewModel, IdAndNickNameUser, Result, ResultType } from '../../model/common'
import { Poll, PollSelection } from '../../model/other'
import { PollVote as CrudVote } from '../../model/other/crud'

import Results from '../results.vue'

@Component({
	props: {
        slug: String
    },
    components: {
        Results
    }
})
export default class Poll extends Vue {
    @State(state => state.vm) vm: PollViewModel
    @ModuleGetter(GET_POLL) poll: Poll
    @ModuleAction(FETCH_POLL) loadPoll: (payload: { slug: string }) => Promise<void>
    @ModuleAction(CREATE_POLL_VOTE) createVote: (payload: { pollId: number, vote: CrudVote }) => Promise<Result[]>

    slug: string

    pollSelection: number = 0

    results: Result[] = []

    loading: boolean = true

    mounted() {
        this.load(this.slug)
    }

    get labels() {
        return this.poll.selections.map((s: PollSelection) => {
            return s.name
        })
    }
    get values() {
        return this.poll.selections.map((s: PollSelection) => {
            return s.votes.length
        })
    }

    get placedVoteId() {
        var vote = this.poll.selections.filter((s: PollSelection) => s.votes.map((v: IdAndNickNameUser) => v.id).includes(this.vm.loggedInUser.id))[0]
        return vote ? vote.id : 0
    }
    get selectedVote() {
        if (this.placedVoteId > 0) {
            return this.poll.selections.filter((s: PollSelection) => s.id == this.placedVoteId)[0]
        } 
    }

    load(slug: string) {
        this.loading = true
        this.loadPoll({ slug: slug }).then(() => this.loading = false)
    }

    vote() {
        this.createVote({ pollId: this.poll.id, vote: {
            pollSelectionId: this.pollSelection,
            votedByUserId: this.vm.loggedInUser.id
        } }).then((results: Result[]) => {
            this.results = results
            this.load(this.poll.slug)
        })
    }

    daysUntilPollCloses(poll: Poll) {
        let today = moment(new Date())
        let created = moment(poll.created)
        return 7 - today.diff(created, 'days')
    }
}
</script>

<style lang="scss" scoped>
a {
    cursor: pointer;
}
.icon-star {
    color: #FFB113;
}
.poll-selection:last-child {
    margin-bottom: 0;
}
</style>
