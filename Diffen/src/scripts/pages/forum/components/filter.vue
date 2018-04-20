<template>
    <div class="card mb-4" :class="{ 'div-disabled': isLoadingPosts }">
        <div class="card-body">
            <h6 class="mb-3">filtrera</h6>
            <hr />
            <div class="list-group mt-3">
                <div class="mb-3">
                    <input id="users" class="form-control form-control-sm" type="text" placeholder="sök på ett nick.." autocomplete="off" />
                    <typeahead v-model="selectedUser" target="#users" :data="users" item-key="value" force-select />
                </div>
                <template v-if="includedUsers.length > 0">
                    <div class="list-group-item flex-column align-items-start" style="background-color: #efefef">
                        <small>visar inlägg av</small>
                    </div>
                    <div class="list-group-item flex-column align-items-start" v-for="includedUser in includedUsers">
                        <div class="d-flex w-100 justify-content-between">
                            <small><strong>{{ includedUser.value }}</strong></small>
                            <button type="button" class="close" v-on:click="removeUser(includedUser)">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                    </div>
                </template>
                <template v-if="excludedUsers && excludedUsers.length > 0">
                    <div class="list-group-item flex-column align-items-start" style="background-color: #efefef">
                        <small>filtrerar bort inlägg av</small>
                    </div>
                    <div class="list-group-item flex-column align-items-start" v-for="excludedUser in excludedUsers">
                        <div class="d-flex w-100 justify-content-between">
                            <small><strong>{{ excludedUser.value }}</strong></small>
                        </div>
                    </div>
                </template>
                <div class="list-group-item flex-column align-items-start">
                    <div class="form-check">
                        <input class="form-check-input" type="radio" id="hasStartingEleven" v-model="startingEleven" value="With">
                        <label class="form-check-label" for="hasStartingEleven">inlägg med startelva</label>
                    </div>
                    <div class="form-check">
                        <input class="form-check-input" type="radio" id="noStartingEleven" v-model="startingEleven" value="Without">
                        <label class="form-check-label" for="noStartingEleven">inlägg utan startelva</label>
                    </div>
                    <div class="form-check">
                        <input class="form-check-input" type="radio" id="allStartingEleven" v-model="startingEleven" value="All">
                        <label class="form-check-label" for="allStartingEleven">både och</label>
                    </div>
                </div>
                <div class="list-group-item flex-column align-items-start">
					<div class="row">
						<div class="col pr-1">
							<date-picker v-model="filter.fromDate" :config="fromDPConfig" placeholder="från" :class="{ 'form-control-sm': true }" @dp-change="dpChange" />
						</div>
						<div class="col pl-1">
							<date-picker v-model="filter.toDate" :config="toDPConfig" placeholder="till" :class="{ 'form-control-sm': true }" @dp-change="dpChange" />
						</div>
					</div>
					<div class="row" v-if="showDatePickerTip">
						<div class="col">
							<div class="alert alert-warning mt-3 mb-0">
								<small class="font-weight-light font-italic">psst.. från-datumet kanske ska vara före till-datumet?</small>
							</div>
						</div>
					</div>
				</div>
            </div>
            <div class="row mt-3">
                <div class="col pr-1">
                    <button class="btn btn-outline-success btn-sm btn-block" :disabled="showDatePickerTip" v-on:click="apply">applicera</button>
                </div>
                <div class="col pl-1">
                    <button class="btn btn-outline-warning btn-sm btn-block" v-on:click="reset">återställ</button>
                </div>
            </div>
            <div class="row mt-3">
                <div class="col">
                    <button class="btn btn-outline-danger btn-sm btn-block" v-on:click="all">visa allt</button>
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component, Watch } from 'vue-property-decorator'
import { Getter, Action, Mutation, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('forum', Getter)
const ModuleAction = namespace('forum', Action)
const ModuleMutation = namespace('forum', Mutation)

const ProfileModuleAction = namespace('profile', Action)

import { 
    GET_IS_LOADING_POSTS,
    GET_FILTER,
    FETCH_PAGED_POSTS,
    SET_IS_LOADING_POSTS,
    SET_FILTER
} from '../../../modules/forum/types'

import { FETCH_KVP_USERS } from '../../../modules/profile/types'

import { StartingEleven, Filter } from '../../../model/forum'
import { ViewModel, KeyValuePair } from '../../../model/common'

import { Typeahead } from 'uiv'
import DatePicker from 'vue-bootstrap-datetimepicker'

@Component({
    components: {
        DatePicker, Typeahead
    }
})
export default class FilterComponent extends Vue {
	@State(state => state.vm) vm: ViewModel
	@ModuleGetter(GET_IS_LOADING_POSTS) isLoadingPosts: boolean
	@ModuleGetter(GET_FILTER) filter: Filter
    @ModuleAction(FETCH_PAGED_POSTS) loadPaged: (payload: { pageNumber: number, pageSize: number, filter: Filter }) => Promise<void>
	@ModuleMutation(SET_IS_LOADING_POSTS) setIsLoadingPosts: (payload: { value: boolean }) => void
	@ModuleMutation(SET_FILTER) setFilter: (payload: { filter: Filter }) => void

	@ProfileModuleAction(FETCH_KVP_USERS) loadUsers: () => Promise<KeyValuePair[]>

	includedUsers: KeyValuePair[] = []
    startingEleven: string = StartingEleven[StartingEleven.All]
    
    showDatePickerTip: boolean = false

	fromDPConfig: any = { 
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
    toDPConfig: any = { 
		format: 'YYYY-MM-DD', 
		useCurrent: false, 
		locale: 'sv', 
		icons: { 
			next: 'icon icon-arrow-right',
			previous: 'icon icon-arrow-left' 
        },
        widgetPositioning: {
            vertical: 'bottom',
            horizontal: 'right'
        }
    }

    users: KeyValuePair[] = []
    selectedUser: any = ''
    
    mounted() {
        this.fetchUsers()
	}

	get excludedUsers(): any {
		return this.filter.excludedUsers
	}
	get current(): Filter {
		return {
            toDate: this.filter.toDate,
            fromDate: this.filter.fromDate,
			startingEleven: StartingEleven[this.startingEleven as keyof typeof StartingEleven],
			includedUsers: this.includedUsers,
			excludedUsers: this.excludedUsers
		}
    }
    
    @Watch('selectedUser')
		onChange() {
			if (this.selectedUser && this.selectedUser.key) {
				this.includedUsers.unshift(this.selectedUser)
				this.users.splice(this.users.indexOf(this.selectedUser), 1)
			}
		}

	apply() {
		this.setFilter({ filter: this.current })
        this.loadPosts()
    }

    reset() {
        this.includedUsers = []
        this.startingEleven = StartingEleven[StartingEleven.All]
        
        this.setFilter({ filter: { excludedUsers: this.vm.loggedInUser.filter.excludedUsers } })
        this.loadPosts()
        this.fetchUsers()
    }

    all() {
        this.includedUsers = []
        this.startingEleven = StartingEleven[StartingEleven.All]
        
        this.setFilter({ filter: {} })
        this.loadPosts()
        this.fetchUsers()
    }

    loadPosts() {
        this.setIsLoadingPosts({ value: true })
        this.loadPaged({ pageNumber: 1, pageSize: this.vm.loggedInUser.filter.postsPerPage, filter: this.filter })
			.then(() => this.setIsLoadingPosts({ value: false }))
    }
    
    dpChange(): void {
        if (this.filter.fromDate && this.filter.toDate) {
            this.filter.fromDate > this.filter.toDate ? this.showDatePickerTip = true : this.showDatePickerTip = false 
        }
    }

    removeUser(selected: KeyValuePair): void {
        this.includedUsers = this.includedUsers.filter((kvp: KeyValuePair) => kvp.key !== selected.key)
		this.users.push(selected)
    }
    
    fetchUsers() {
        this.loadUsers().then((users: KeyValuePair[]) => this.users = users)
    }
}
</script>