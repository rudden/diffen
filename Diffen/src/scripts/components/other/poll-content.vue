<template>
	<div class="container">
        <div class="row">
            <div class="col pl-0 pr-0">
                <ul class="list-unstyled list-spaced mb-0" style="width: 100%">
                    <li v-for="selection in poll.selections" :key="selection.name" class="poll-selection">
                        <span class="badge badge-primary float-right">{{ selection.votes.length }}</span>
                        <template v-if="poll.isOpen && !placedVote">
                            <div class="form-check">
                                <input class="form-check-input" type="radio" v-model="pollSelection" :id="selection.id" :value="selection.id">
                                <label class="form-check-label" :for="selection.id">{{ selection.name }}</label>
                            </div>
                        </template>
                        <template v-else>
                            <span>
                                {{ selection.name }} 
                                <span class="icon icon-star" v-if="selection.isWinner"></span> 
                                <span class="icon icon-check" v-if="placedVote && selection.id == placedVote.id"></span>
                            </span>
                        </template>
                    </li>
                </ul>
                <results :items="results" class="pt-3" />
            </div>
        </div>
        <div class="row mt-3" v-if="poll.isOpen && !placedVote">
            <div class="col pl-0 pr-0">
                <button class="btn btn-success btn-sm btn-block" v-on:click="vote" :disabled="pollSelection == 0">Rösta</button>
            </div>
        </div>
	</div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Action, State, namespace } from 'vuex-class'

const ModuleAction = namespace('other', Action)

import { CREATE_POLL_VOTE } from '../../modules/other/types'

import { PageViewModel, IdAndNickNameUser, Result, ResultType } from '../../model/common'
import { Poll, PollSelection } from '../../model/other'
import { PollVote as CrudVote } from '../../model/other/crud'

import Results from '../results.vue'

@Component({
	props: {
        poll: Object,
    },
    components: {
        Results
    }
})
export default class PollContent extends Vue {
    @State(state => state.vm) vm: PageViewModel
    @ModuleAction(CREATE_POLL_VOTE) createVote: (payload: { pollId: number, vote: CrudVote }) => Promise<Result[]>

    poll: Poll

    pollSelection: number = 0

    results: Result[] = []

    get placedVote() {
        var vote = this.poll.selections.filter((s: PollSelection) => s.votes.map((v: IdAndNickNameUser) => v.id).includes(this.vm.loggedInUser.id))[0]
        return vote ? vote : null
    }

    vote() {
        this.createVote({ pollId: this.poll.id, vote: {
            pollSelectionId: this.pollSelection,
            votedByUserId: this.vm.loggedInUser.id
        } }).then((results: Result[]) => this.results = results)
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
