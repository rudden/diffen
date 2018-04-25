<template>
    <ul class="list-group media-list media-list-stream">
        <li class="list-group-item" :class="{ 'p-3': isSmall, 'p-4': !isSmall }">
            <modal v-bind="{ id: 'new-poll', header: 'ny omröstning' }">
                <template slot="btn">
                    <button class="btn btn-sm btn-primary" :class="{ 'float-right': !isSmall, 'btn-block': isSmall }" data-toggle="modal" data-target="#new-poll">ny poll</button>
                </template>
                <template slot="body">
                    <template v-if="creating">
                        <loader v-bind="{ background: '#699ED0' }" />
                    </template>
                    <template v-else>
                        <div class="row">
                            <div class="col">
                                <div class="form-group row">
                                    <label for="name" class="col-sm-2 col-form-label">titel</label>
                                    <div class="col-sm-10">
                                        <input type="text" id="name" v-model="newPoll.name" class="form-control form-control-sm" placeholder="namn på omröstningen" />
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <label for="selection" class="col-sm-2 col-form-label">alternativ</label>
                                    <div class="col-sm-10">
                                        <input type="text" id="selection" v-model="newSelection" class="form-control form-control-sm" :placeholder="selectionPlaceholder" v-on:keyup.enter="addToSelections" :disabled="maxNumberOfSelections" />
                                    </div>
                                </div>
                                <div class="form-group">
                                </div>
                                <template v-if="newPoll.selections.length > 0">
                                    <hr />
                                    <ul class="list-unstyled list-spaced mb-3">
                                        <li><strong>alternativ</strong></li>
                                        <li v-for="selection in newPoll.selections" :key="selection">
                                            <a class="float-right" v-on:click="removeSelection(selection)">
                                                <span class="icon icon-trash"></span>
                                            </a>
                                            <span>{{ selection }}</span>
                                        </li>
                                    </ul>
                                </template>
                            </div>
                        </div>
                        <results :items="results" class="pb-3" />
                        <div class="row">
                            <div class="col">
                                <button class="btn btn-success btn-sm btn-block" v-on:click="create" :disabled="!canCreate">skapa</button>
                            </div>
                        </div>
                    </template>
                </template>
            </modal>
            <h4 class="mb-0" v-if="!isSmall">omröstningar</h4>
        </li>
        <li class="media list-group-item" :class="{ 'p-3': isSmall, 'p-4': !isSmall }" v-show="loading">
            <loader v-bind="{ background: '#699ED0' }" />
        </li>
        <div v-show="!loading">
            <li class="list-group-item media" :class="{ 'p-3': isSmall, 'p-4': !isSmall }" v-for="poll in polls" :key="poll.id">
                <poll-component :poll="poll" :is-small="isSmall" />
            </li>
        </div>
    </ul>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Action, Mutation, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('other', Getter)
const ModuleAction = namespace('other', Action)
const ModuleMutation = namespace('other', Mutation)

import { PageViewModel, Result, ResultType } from '../../model/common'
import { Poll, PollSelection } from '../../model/other'
import { Poll as CrudPoll } from '../../model/other/crud'

import { GET_POLLS, FETCH_POLLS, FETCH_ACTIVE_POLLS, CREATE_POLL } from '../../modules/other/types'

import PollComponent from './poll.vue'
import Modal from '../modal.vue'
import Results from '../results.vue'
import { Stretch as Loader } from 'vue-loading-spinner'

@Component({
    props: {
        typeOfPolls: {
            type: String,
            default: 'all'
        },
        isSmall: {
            type: Boolean,
            default: false
        }
    },
	components: {
        Loader, PollComponent, Modal, Results
	}
})
export default class Polls extends Vue {
    @State(state => state.vm) vm: PageViewModel
    @ModuleGetter(GET_POLLS) polls: Poll[]
    @ModuleAction(FETCH_POLLS) loadPolls: () => Promise<void>
	@ModuleAction(FETCH_ACTIVE_POLLS) loadActivePolls: (payload: { amount: number }) => Promise<void>
    @ModuleAction(CREATE_POLL) createPoll: (payload: { poll: CrudPoll }) => Promise<Result[]>

    typeOfPolls: string
    isSmall: boolean

    loading: boolean = false
    creating: boolean = false

    newPoll: CrudPoll = new CrudPoll()
    newSelection: string = ''

    results: Result[] = []

	mounted() {
        this.load()
        this.newPoll.createdByUserId = this.vm.loggedInUser.id
    }

    get canCreate(): boolean {
        return this.newPoll.name && this.newPoll.selections.length > 1 ? true : false
    }

    get maxNumberOfSelections(): boolean {
        return this.newPoll.selections.length == 10 ? true : false
    }

    get selectionPlaceholder(): string {
        return this.maxNumberOfSelections ? 'max antal alternativ' : 'nytt alternativ'
    }

    load() {
        switch (this.typeOfPolls) {
            case 'all':
                this.loadAllPolls()
                break
            case 'active':
                this.loadAllActivePolls()
                break
        }
    }

    loadAllPolls() {
        this.loading = true
		this.loadPolls().then(() => this.loading = false)
    }

    loadAllActivePolls() {
        this.loading = true
		this.loadActivePolls({ amount: 5 }).then(() => this.loading = false)
    }

    addToSelections() {
        if (!this.newPoll.selections.includes(this.newSelection)) {
            this.newPoll.selections.push(this.newSelection)
            this.newSelection = ''
        }
    }

    removeSelection(selection: string) {
        this.newPoll.selections = this.newPoll.selections.filter((s: string) => s !== selection)
    }

    create() {
        this.creating = true
        this.createPoll({ poll: this.newPoll })
            .then((results: Result[]) => {
                this.results = results
                this.creating = false
                this.newPoll = new CrudPoll()
                this.load()
            })
    }
}
</script>

<style lang="scss" scoped>
a {
    cursor: pointer;
}
</style>