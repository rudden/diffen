<template>
    <div>
        <div class="card mb-4" v-if="isSmall">
            <div class="card-body">
                <new-poll :on-modal-close="load" />
                <h6 class="mb-0">Omröstningar</h6>
                <hr />
                <template v-if="!loading">
                    <ul class="list-unstyled list-spaced mb-0">
                        <li class="ellipsis" v-for="poll in filtered" :key="poll.id">
                            <template v-if="openInModal">
                                <div class="media-heading">
                                    <modal v-bind="{ attributes: { name: `poll-${poll.id}` }, header: poll.name, button: { classes: 'small on-click', text: poll.name }, onClose: () => isCopied = false }">
                                        <template slot="body">
                                            <poll-component :slug="poll.slug" :nested-in-modal="true" />
                                        </template>
                                        <template slot="footer">
                                            <button class="btn btn-block btn-sm btn-primary" v-clipboard="`${pollUrl}/${poll.slug}`" v-on:click="isCopied = true" :disabled="isCopied">
                                                {{ isCopied ? `Kopierade ${pollUrl}/${poll.slug}` : 'Kopiera länk till omröstningen' }}
                                            </button>
                                        </template>
                                    </modal>
                                </div>
                            </template>
                        </li>
                    </ul>
                </template>
                <template v-else>
                    <loader v-bind="{ background: '#699ED0' }" />
                </template>
            </div>
        </div>
        <ul class="list-group media-list media-list-stream" v-else>
            <li class="list-group-item p-4">
                <new-poll :on-modal-close="load" />
                <h4 class="mb-0">Omröstningar</h4>
            </li>
            <li class="media list-group-item p-4" v-if="loading">
                <loader v-bind="{ background: '#699ED0' }" />
            </li>
            <template v-else>
                <li class="media list-group-item p-4" v-if="typeOfPolls == 'all'">
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
                            <input class="form-check-input" type="radio" v-model="pollsFilter" id="my" value="My">
                            <label class="form-check-label" for="my">Mina</label>
                        </div>
                        <div class="form-check form-check-inline">
                            <input class="form-check-input" type="radio" v-model="pollsFilter" id="all" value="All">
                            <label class="form-check-label" for="all">Alla</label>
                        </div>
                    </div>
                </li>
                <template v-if="filtered.length > 0">
                    <li class="list-group-item media p-4" v-for="poll in filtered" :key="poll.id">
                        <span class="icon icon-hand text-muted mr-2"></span>
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
                                            <poll-component :slug="poll.slug" />
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
                </template>
                <template v-else>
                    <li class="list-group-item media p-4">
                        <div class="col pl-0 pr-0">
                            <div class="alert alert-warning mb-0">Hittade inga omröstningar</div>
                        </div>
                    </li>
                </template>
            </template>
        </ul>
    </div>
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

import NewPoll from './new-poll.vue'
import PollComponent from './poll.vue'
import Results from '../results.vue'
import Modal from '../modal.vue'

enum PollFilter {
    Open, Closed, My, All
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
        PollComponent, Results, Modal, NewPoll
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
                case PollFilter.My:
                    this.filteredPolls = this.polls.filter((p: Poll) => p.byUser.id == this.vm.loggedInUser.id)
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
}
</script>

<style lang="scss" scoped>
a {
    cursor: pointer;
}
</style>