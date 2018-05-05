<template>
    <ul class="list-group media-list media-list-stream">
        <li class="media list-group-item p-4" v-show="loading">
            <loader v-bind="{ background: '#699ED0' }" />
        </li>
        <div v-show="!loading">
            <li class="list-group-item p-4">
                <span class="text-muted float-right">
                    <span class="badge" :class="{ 'badge-warning': poll.isOpen, 'badge-danger': !poll.isOpen }">
                        {{ poll.isOpen ? `stänger om ${daysUntilPollCloses(poll)} dagar` : 'stängd' }}
                    </span>
                </span>
                <h4 class="mb-0">{{ poll.name }}</h4>
            </li>
            <li class="list-group-item media p-4">
                <poll-content :poll="poll" />
            </li>
        </div>
    </ul>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Action, Mutation, State, namespace } from 'vuex-class'

import moment from 'moment'

const ModuleGetter = namespace('other', Getter)
const ModuleAction = namespace('other', Action)
const ModuleMutation = namespace('other', Mutation)

import { GET_POLL, FETCH_POLL } from '../../modules/other/types'

import { PollViewModel } from '../../model/common'
import { Poll } from '../../model/other'

import PollContent from './poll-content.vue'

@Component({
    components: {
        PollContent
    }
})
export default class PollComponent extends Vue {
    @State(state => state.vm) vm: PollViewModel
    @ModuleGetter(GET_POLL) poll: Poll
    @ModuleAction(FETCH_POLL) loadPoll: (payload: { slug: string }) => Promise<void>

    loading: boolean = true

    mounted() {
        this.loadPoll({ slug: this.vm.selectedPollSlug }).then(() => this.loading = false)
    }
        
    daysUntilPollCloses(poll: Poll) {
        let today = moment(new Date())
        let created = moment(poll.created)
        return 7 - today.diff(created, 'days')
    }
}
</script>
