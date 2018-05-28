<template>
    <div class="card pl-2 pr-2" :class="{ 'div-disabled': isLoadingPosts }">
        <div class="card-body">
            <!-- <template v-if="isLoadingPosts">
    			<loader v-bind="{ background: '#699ED0' }" />
            </template>
            <template v-else> -->
                <h6 class="mb-0">
                    <span class="badge badge-dark float-right badge-pill">
                        Visar {{ pagedPosts.data.length }} {{ pagedPosts.data.length !== pagedPosts.total ? `av ${pagedPosts.total} inlägg` : ' inlägg' }}
                    </span>
                    <span style="cursor: pointer" @click="show = !show" v-tooltip.right="tooltipText">
                        Filtrera
                        <span class="icon ml-1" :class="{ 'icon-chevron-small-down': !show, 'icon-chevron-small-up': show }"></span>
                    </span>
                </h6>
                <template v-if="show">
                    <hr class="mt-3" />
                    <div class="list-group mt-3">
                        <div class="mb-3">
                            <input id="users" class="form-control form-control-sm" type="text" placeholder="sök på ett nick.." autocomplete="off" />
                            <typeahead v-model="selectedUser" target="#users" :data="users" item-key="value" force-select />
                        </div>
                        <template v-if="includedUsers.length > 0">
                            <div class="list-group-item flex-column align-items-start" style="background-color: #efefef">
                                <small>Visar inlägg av</small>
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
                                <small>Filtrerar bort inlägg av</small>
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
                                <label class="form-check-label" for="hasStartingEleven">Inlägg med startelva</label>
                            </div>
                            <div class="form-check">
                                <input class="form-check-input" type="radio" id="noStartingEleven" v-model="startingEleven" value="Without">
                                <label class="form-check-label" for="noStartingEleven">Inlägg utan startelva</label>
                            </div>
                            <div class="form-check">
                                <input class="form-check-input" type="radio" id="allStartingEleven" v-model="startingEleven" value="All">
                                <label class="form-check-label" for="allStartingEleven">Både och</label>
                            </div>
                        </div>
                        <div class="list-group-item flex-column align-items-start">
                            <div class="form-group mb-0">
                                <input type="text" class="form-control form-control-sm" v-model="filter.messageWildCard" placeholder="sök på inläggsinnehåll" />
                            </div>
                        </div>
                        <div class="list-group-item flex-column align-items-start">
                            <div class="row col">
                                <div class="form-check form-check-inline" v-for="thread in ongoingThreads" :key="thread.id">
                                    <input class="form-check-input" type="checkbox" :id="thread.id" :value="thread.id" v-model="filter.threadIds">
                                    <label class="form-check-label" :for="thread.id">{{ thread.name }} {{ thread.numberOfPosts > 0 ? `(${thread.numberOfPosts})` : '' }}</label>
                                </div>
                            </div>
                            <template v-if="plannedThreads.length > 0">
                                <hr />
                                <div class="row col">
                                    <div class="form-check form-check-inline" v-for="thread in plannedThreads" :key="thread.id">
                                        <input class="form-check-input" type="checkbox" :id="thread.id" :value="thread.id" v-model="filter.threadIds">
                                        <label class="form-check-label" :for="thread.id">{{ thread.name }} {{ thread.numberOfPosts > 0 ? `(${thread.numberOfPosts})` : '' }}</label>
                                    </div>
                                </div>
                            </template>
                        </div>
                        <div class="list-group-item flex-column align-items-start">
                            <div class="row">
                                <div class="col pr-1">
                                    <date-picker v-model="filter.fromDate" :config="fromDPConfig" placeholder="från" :class="{ 'form-control-sm': true }" />
                                </div>
                                <div class="col pl-1">
                                    <date-picker v-model="filter.toDate" :config="toDPConfig" placeholder="till" :class="{ 'form-control-sm': true }" />
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
                            <button class="btn btn-outline-success btn-sm btn-block" :disabled="showDatePickerTip" v-on:click="apply">Applicera</button>
                        </div>
                        <div class="col pl-1">
                            <button class="btn btn-outline-warning btn-sm btn-block" v-on:click="reset">Återställ</button>
                        </div>
                    </div>
                    <div class="row mt-3">
                        <div class="col">
                            <button class="btn btn-outline-danger btn-sm btn-block" v-on:click="all">Visa allt</button>
                        </div>
                    </div>
                </template>
            <!-- </template> -->
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
    GET_THREADS,
    GET_PAGED_POSTS,
    GET_IS_LOADING_POSTS,
    GET_FILTER,
    FETCH_PAGED_POSTS,
    SET_IS_LOADING_POSTS,
    SET_FILTER
} from '../../../modules/forum/types'

import { FETCH_KVP_USERS } from '../../../modules/profile/types'

import { StartingEleven, Filter, Post, Thread, ThreadType } from '../../../model/forum'
import { PageViewModel, KeyValuePair, Paging } from '../../../model/common'

import { Typeahead } from 'uiv'
import DatePicker from 'vue-bootstrap-datetimepicker'

@Component({
    components: {
        DatePicker, Typeahead
    }
})
export default class FilterComponent extends Vue {
    @State(state => state.vm) vm: PageViewModel
    @ModuleGetter(GET_PAGED_POSTS) pagedPosts: Paging<Post>
	@ModuleGetter(GET_IS_LOADING_POSTS) isLoadingPosts: boolean
    @ModuleGetter(GET_FILTER) filter: Filter
	@ModuleGetter(GET_THREADS) threads: Thread[]
    @ModuleAction(FETCH_PAGED_POSTS) loadPaged: (payload: { pageNumber: number, pageSize: number, filter: Filter }) => Promise<void>
	@ModuleMutation(SET_IS_LOADING_POSTS) setIsLoadingPosts: (payload: { value: boolean }) => void
	@ModuleMutation(SET_FILTER) setFilter: (payload: { filter: Filter }) => void

	@ProfileModuleAction(FETCH_KVP_USERS) loadUsers: () => Promise<KeyValuePair[]>

	includedUsers: KeyValuePair[] = []
    startingEleven: string = StartingEleven[StartingEleven.All]
    show: boolean = false

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
            messageWildCard: this.filter.messageWildCard,
			startingEleven: StartingEleven[this.startingEleven as keyof typeof StartingEleven],
			includedUsers: this.includedUsers,
            excludedUsers: this.excludedUsers,
            threadIds: this.filter.threadIds
		}
    }
    get showDatePickerTip() {
        if (this.filter.fromDate && this.filter.toDate)
            return this.filter.fromDate > this.filter.toDate ? true : false
        return false
    }

    get tooltipText() {
        return !this.show ? 'Klicka här för att filtrera forumets innehåll' : 'Klicka här för att dölja filter-komponenten'
    }
    get ongoingThreadType() {
        return ThreadType.Ongoing
    }
    get plannedThreadType() {
        return ThreadType.Planned
    }
    get ongoingThreads() {
        return this.threads.filter((t: Thread) => t.type == ThreadType.Ongoing)
    }
    get plannedThreads() {
        return this.threads.filter((t: Thread) => t.type == ThreadType.Planned && t.startTime && new Date(t.startTime) >= new Date())
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

        this.setFilter({ filter: { excludedUsers: this.vm.loggedInUser.filter.excludedUsers, threadIds: [] } })
        this.loadPosts()
        this.fetchUsers()
    }

    all() {
        this.includedUsers = []
        this.startingEleven = StartingEleven[StartingEleven.All]
        
        this.setFilter({ filter: { threadIds: [] } })
        this.loadPosts()
        this.fetchUsers()
    }

    loadPosts() {
        this.setIsLoadingPosts({ value: true })
        this.loadPaged({ pageNumber: 1, pageSize: this.vm.loggedInUser.filter.postsPerPage, filter: this.filter })
			.then(() => this.setIsLoadingPosts({ value: false }))
    }

    removeUser(selected: KeyValuePair): void {
        this.includedUsers = this.includedUsers.filter((kvp: KeyValuePair) => kvp.key !== selected.key)
		this.users.push(selected)
    }
    
    fetchUsers() {
        this.loadUsers().then((users: KeyValuePair[]) => this.users = users)
    }

    getThreadNames() {
        if (this.filter.threadIds) {
            let threadNamesArray = this.threads.filter((t: Thread) => this.filter.threadIds ? this.filter.threadIds.includes(t.id) : false)
            if (threadNamesArray) {
                return threadNamesArray.map((t: Thread) => t.name).join(', ')
            }
        }
        return ''
    }
}
</script>