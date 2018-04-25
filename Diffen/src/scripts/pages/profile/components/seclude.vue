<template>
    <div>
        <template v-if="loggedInUserIsAdmin && !selectedUserIsScissorOrAdmin">
            <modal v-bind="{ id: `seclude-${user.id}`, header: 'spärra användaren' }">
                <template slot="btn">
                    <a href="#" data-toggle="modal" :data-target="'#' + `seclude-${user.id}`">
                        <span class="badge badge-pill badge-danger">{{ user.secludedUntil? `spärrad till ${user.secludedUntil}` : 'spärra' }}</span>
                    </a>
                </template>
                <template slot="body">
                    <template v-if="!loading">
                        <div class="form-group">
                            <date-picker v-model="date" :config="dpConfig" />
                        </div>
                        <results :items="results" class="mb-3" />
                        <div class="form-group mb-0">
                            <div class="row">
                                <div class="col">
                                    <button type="button" class="btn btn-sm btn-success btn-block" v-on:click="seclude">spara</button>
                                </div>
                            </div>
                        </div>
                    </template>
                    <template v-else>
                        <loader v-bind="{ background: '#699ED0' }" />
                    </template>
                </template>
            </modal>
        </template>
        <template v-else>
            <p v-if="user.secludedUntil">
                <span class="badge badge-pill badge-danger">spärrad till {{ user.secludedUntil }}</span>
            </p>
        </template>
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Action, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('profile', Getter)
const ModuleAction = namespace('profile', Action)

import { User } from '../../../model/profile/'
import { User as CrudUser } from '../../../model/profile/crud'
import { Player } from '../../../model/squad/'
import { PageViewModel, Result, ResultType } from '../../../model/common'

import Modal from '../../../components/modal.vue'
import Results from '../../../components/results.vue'
import { Stretch as Loader } from 'vue-loading-spinner'
import DatePicker from 'vue-bootstrap-datetimepicker'

import {
    GET_USER,
    FETCH_USER,
    SECLUDE_USER
} from '../../../modules/profile/types'

import {
    GET_PLAYERS,
    FETCH_PLAYERS
} from '../../../modules/squad/types'

@Component({
    components: {
        Results, Loader, DatePicker, Modal
    }
})
export default class Seclude extends Vue {
	@State(state => state.vm) vm: PageViewModel
	@ModuleGetter(GET_USER) user: User
	@ModuleAction(FETCH_USER) loadUser: (payload: { id: string }) => Promise<void>
    @ModuleAction(SECLUDE_USER) secludeUser: (payload: { userId: string, to: string }) => Promise<Result[]>

    results: Result[] = []

    loading: boolean = false

    date: Date = new Date()
    dpConfig: any = { 
		format: 'YYYY-MM-DD', 
		useCurrent: false, 
		locale: 'sv', 
		icons: { 
			next: 'icon icon-arrow-right',
			previous: 'icon icon-arrow-left' 
        },
        widgetPositioning: {
            vertical: 'bottom',
            horizontal: 'left'
        }
    }

    mounted() {
		if (this.user.secludedUntil) {
			this.date = new Date(this.user.secludedUntil)
		}
	}

    get loggedInUserIsAdmin(): boolean {
        return this.vm.loggedInUser.inRoles.some(role => role == 'Admin')
    }
    get selectedUserIsScissorOrAdmin(): boolean {
        return this.user.inRoles.some(role => role == 'Admin' || role == 'Scissor')
    }

    seclude() {
        this.loading = true
        this.secludeUser({ userId: this.user.id, to: this.date ? this.date.toString() : '' })
            .then((results: Result[]) => {
                this.loadUser({ id: this.user.id })
                this.results = results
                this.loading = false
            })
    }
}
</script>

<style lang="scss" scoped>
.edit-btn {
    color: #699ED0; 
    font-size: 150%; 
    cursor: pointer; 
    display: block; 
    margin-top: 20px;
}
</style>
