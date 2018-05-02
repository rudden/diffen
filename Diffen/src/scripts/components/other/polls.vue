<template>
    <ul class="list-group media-list media-list-stream">
        <li class="list-group-item" :class="{ 'p-3': isSmall, 'p-4': !isSmall }">
            <modal v-bind="modalAttributes.newPoll">
                <template slot="body">
                    <template v-if="creating">
                        <loader v-bind="{ background: '#699ED0' }" />
                    </template>
                    <template v-else>
                        <div class="row">
                            <div class="col">
                                <div class="form-group row">
                                    <label for="name" class="col-sm-2 col-form-label">Titel</label>
                                    <div class="col-sm-10">
                                        <input type="text" id="name" v-model="newPoll.name" class="form-control form-control-sm" placeholder="Namn på omröstningen" />
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <label for="selection" class="col-sm-2 col-form-label">Alternativ</label>
                                    <div class="col-sm-10">
                                        <input type="text" id="selection" v-model="newSelection" class="form-control form-control-sm" :placeholder="selectionPlaceholder" v-on:keyup.enter="addToSelections" :disabled="maxNumberOfSelections" />
                                    </div>
                                </div>
                                <div class="form-group">
                                </div>
                                <template v-if="newPoll.selections.length > 0">
                                    <hr />
                                    <ul class="list-unstyled list-spaced mb-3">
                                        <li><strong>Alternativ</strong></li>
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
                                <button class="btn btn-success btn-sm btn-block" v-on:click="create" :disabled="!canCreate">Skapa</button>
                            </div>
                        </div>
                    </template>
                </template>
            </modal>
            <h4 class="mb-0" v-if="!isSmall">Omröstningar</h4>
        </li>
        <li class="media list-group-item" :class="{ 'p-3': isSmall, 'p-4': !isSmall }" v-show="loading">
            <loader v-bind="{ background: '#699ED0' }" />
        </li>
        <div v-show="!loading">
            <li class="media list-group-item p-4" v-if="!isSmall && typeOfPolls == 'all'">
                <div class="col pl-0 pr-0">
                    <div class="form-group float-right mb-0">
                        <input type="text" class="form-control form-control-sm" v-model="pollSearch" placeholder="Sök">
                    </div>
                    <div class="form-check form-check-inline">
                        <input class="form-check-input" type="radio" v-model="pollsFilter" id="open" value="Open">
                        <label class="form-check-label" for="open">Öppna</label>
                    </div>
                    <div class="form-check form-check-inline">
                        <input class="form-check-input" type="radio" v-model="pollsFilter" id="closed" value="Closed">
                        <label class="form-check-label" for="closed">Stängda</label>
                    </div>
                    <div class="form-check form-check-inline">
                        <input class="form-check-input" type="radio" v-model="pollsFilter" id="all" value="All">
                        <label class="form-check-label" for="all">Alla</label>
                    </div>
                </div>
            </li>
            <template v-if="filtered.length > 0">
                <li class="list-group-item media" :class="{ 'p-3': isSmall, 'p-4': !isSmall }" v-for="poll in filtered" :key="poll.id">
                    <span class="icon icon-hand text-muted mr-2" v-if="!isSmall"></span>
                    <div class="media-body">
                        <span class="text-muted float-right">
                            <span class="badge" :class="{ 'badge-success': poll.isOpen, 'badge-danger': !poll.isOpen }">
                                {{ poll.isOpen ? 'öppen' : 'stängd' }}
                            </span>
                        </span>
                        <template v-if="openInModal">
                            <div class="media-heading">
                                <modal v-bind="{ attributes: { name: `poll-${poll.id}` }, header: poll.name, button: { classes: 'font-weight-bold small on-click', text: poll.name }, onClose: () => isCopied = false }">
                                    <template slot="body">
                                        <poll-content :poll="poll" />
                                    </template>
                                    <template slot="footer">
                                        <button class="btn btn-block btn-sm btn-primary" v-clipboard="`${pollUrl}/${poll.slug}`" v-on:click="isCopied = true" :disabled="isCopied">
                                            {{ isCopied ? `Kopierade ${pollUrl}/${poll.slug}` : 'Kopiera länk till omröstningen' }}
                                        </button>
                                    </template>
                                </modal>
                            </div>
                        </template>
                        <template v-else>
                            <a :href="`/omrostning/${poll.slug}`">{{ poll.name }}</a>
                        </template>
                    </div>
                </li>
                <template v-if="isSmall">
                    <li class="list-group-item p-0">
                        <a href="/polls" class="btn btn-sm btn-primary btn-block btn__no-top-radius">Visa fler</a>
                    </li>
                </template>
            </template>
            <template v-else>
                <li class="list-group-item media p-4">
                    <div class="col pl-0 pr-0">
                        <div class="alert alert-warning mb-0">Hittade inga polls</div>
                    </div>
                </li>
            </template>
        </div>
    </ul>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component, Watch } from 'vue-property-decorator'
import { Getter, Action, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('other', Getter)
const ModuleAction = namespace('other', Action)

import { PageViewModel, Result, ResultType } from '../../model/common'
import { Poll, PollSelection } from '../../model/other'
import { Poll as CrudPoll } from '../../model/other/crud'

import { GET_POLLS, FETCH_POLLS, FETCH_ACTIVE_POLLS, CREATE_POLL } from '../../modules/other/types'

import PollContent from './poll-content.vue'
import Results from '../results.vue'
import Modal from '../modal.vue'

enum PollFilter {
    Open, Closed, All
}

@Component({
    props: {
        typeOfPolls: {
            type: String,
            default: 'all'
        },
        isSmall: {
            type: Boolean,
            default: false
        },
        openInModal: {
            type: Boolean,
            default: true
        }
    },
	components: {
        PollContent, Results, Modal
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
    isCopied: boolean = false

    newPoll: CrudPoll = new CrudPoll()
    newSelection: string = ''

    results: Result[] = []

    modalAttributes: any = {
        newPoll: {
            attributes: {
                name: 'new-poll'
            },
            header: 'Ny omröstning',
            button: {
                classes: `btn btn-sm btn-success ${this.isSmall ? 'btn-block' : 'float-right'}`,
                text: 'Skapa ny omröstning'
            }
        }
    }

    pollsFilter: string = ''
    filteredPolls: Poll[] = []
	pollSearch: string = ''

    private pollUrl: string = ''

	mounted() {
        var getUrl = window.location
        this.pollUrl = getUrl .protocol + '//' + getUrl.host + '/' + getUrl.pathname.split('/')[0] + 'omrostning'

        this.newPoll.createdByUserId = this.vm.loggedInUser.id

        this.load()
    }

    get canCreate(): boolean {
        return this.newPoll.name && this.newPoll.selections.length > 1 ? true : false
    }

    get maxNumberOfSelections(): boolean {
        return this.newPoll.selections.length == 10 ? true : false
    }

    get selectionPlaceholder(): string {
        return this.maxNumberOfSelections ? 'Max antal alternativ' : 'Nytt alternativ'
    }

    @Watch('pollsFilter')
        onChange() {
            let selected = PollFilter[this.pollsFilter as keyof typeof PollFilter]
            switch (selected) {
                case PollFilter.Open:
                    this.filteredPolls = this.polls.filter((p: Poll) => p.isOpen)
                    break
                case PollFilter.Closed:
                    this.filteredPolls = this.polls.filter((p: Poll) => !p.isOpen)
                    break
                case PollFilter.All:
                    this.filteredPolls = this.polls
                    break
            }
        }

    get filtered() {
		return this.filteredPolls.filter((p: Poll) => {
			return p.name.toLowerCase().includes(this.pollSearch.toLowerCase())
		})
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
        this.loadPolls()
            .then(() => {
                this.pollsFilter = PollFilter[PollFilter.Open]
                this.loading = false
            })
    }

    loadAllActivePolls() {
        this.loading = true
        this.loadActivePolls({ amount: 5 })
            .then(() => {
                this.filteredPolls = this.polls
                this.loading = false
            })
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