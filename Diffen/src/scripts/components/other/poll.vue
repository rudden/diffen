<template>
	<div style="display: contents">
        <span class="icon icon-hand text-muted mr-2" v-if="!isSmall"></span>
        <div class="media-body">
            <span class="text-muted float-right">
                <span class="badge" :class="{ 'badge-success': poll.isOpen, 'badge-danger': !poll.isOpen }">
                    {{ poll.isOpen ? 'öppen' : 'stängd' }}
                </span>
            </span>
            <div class="media-heading">
                <modal v-bind="{ id: `poll-${poll.id}`, header: poll.name }">
                    <template slot="btn">
                        <a href="#" data-toggle="modal" :data-target="'#' + `poll-${poll.id}`">
                            <template v-if="isSmall">
                                <small><strong>{{ poll.name }}</strong></small>
                            </template>
                            <template v-else>
                                <strong>{{ poll.name }}</strong>
                            </template>
                        </a>
                    </template>
                    <template slot="body">
                        <template v-if="voting">
                            <loader v-bind="{ background: '#699ED0' }" />
                        </template>
                        <template v-else>
                            <ul class="list-unstyled list-spaced mb-0">
                                <li v-for="selection in poll.selections" :key="selection.name">
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
                            <div class="row mt-3" v-if="poll.isOpen && !placedVote">
                                <div class="col">
                                    <button class="btn btn-success btn-sm btn-block" v-on:click="vote" :disabled="pollSelection == 0">rösta</button>
                                </div>
                            </div>
                        </template>
                    </template>
                </modal>
            </div>
        </div>
	</div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Action, Mutation, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('other', Getter)
const ModuleAction = namespace('other', Action)
const ModuleMutation = namespace('other', Mutation)

import { CREATE_POLL_VOTE } from '../../modules/other/types'

import { PageViewModel, IdAndNickNameUser, Result, ResultType } from '../../model/common'
import { Poll, PollSelection } from '../../model/other'
import { PollVote as CrudVote } from '../../model/other/crud'

import Modal from '../modal.vue'
import Results from '../results.vue'
import { Stretch as Loader } from 'vue-loading-spinner'

@Component({
	props: {
        poll: Object,
        isSmall: {
            type: Boolean,
            default: false
        }
    },
    components: {
        Modal, Results, Loader
    }
})
export default class PollComponent extends Vue {
    @State(state => state.vm) vm: PageViewModel
    @ModuleAction(CREATE_POLL_VOTE) createVote: (payload: { pollId: number, vote: CrudVote }) => Promise<Result[]>

    poll: Poll
    isSmall: boolean

    pollSelection: number = 0

    results: Result[] = []

    voting: boolean = false

    get placedVote() {
        var vote = this.poll.selections.filter((s: PollSelection) => s.votes.map((v: IdAndNickNameUser) => v.id).includes(this.vm.loggedInUser.id))[0]
        return vote ? vote : null
    }

    vote() {
        this.voting = true
        this.createVote({ pollId: this.poll.id, vote: {
            pollSelectionId: this.pollSelection,
            votedByUserId: this.vm.loggedInUser.id
        } }).then((results: Result[]) => {
            this.results = results
            this.voting = false
        })
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
</style>
