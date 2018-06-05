<template>
	<div class="container container__profile mt-3 mb-5">
		<div class="card">
			<div class="card-header">Forumfilter</div>
			<div class="card-body">
				<div class="row" v-if="!loading">
					<div class="col">
						<div class="form-group">
							<div class="mb-3">
								<input id="filter-users" class="form-control form-control-sm" type="text" placeholder="Sök på ett nick.." autocomplete="off" />
								<typeahead v-model="selectedUser" target="#filter-users" :data="users" item-key="value" force-select />
							</div>
							<div class="list-group" v-if="excludedUsers.length > 0">
								<div class="list-group-item p-2" style="background-color: #efefef">
									Användare vars inlägg filtreras bort
								</div>
								<div class="list-group-item flex-column align-items-start p-2" v-for="excludedUser in excludedUsers" :key="excludedUser.key">
									<div class="d-flex w-100 justify-content-between">
										<a style="color: black; font-weight: bold" v-bind:href="'/profil/' + excludedUser.key">{{ excludedUser.value }}</a>
										<button type="button" class="close" v-on:click="removeUser(excludedUser)">
											<span aria-hidden="true">&times;</span>
										</button>
									</div>
								</div>
							</div>
						</div>
						<div class="form-group">
							<div class="row">
								<div class="col-lg-2 col-md-2 col-sm-12 col-xs-12 pr-0">
									<p class="mb-0">Inlägg per sida</p>
								</div>
								<div class="col">
									<input type="number" v-model="pageSize" class="form-control form-control-sm" min="1" max="50" />
								</div>
							</div>
						</div>
						<div class="form-group">
							<div class="row">
								<div class="col-lg-2 col-md-2 col-sm-12 col-xs-12 pr-0">
									<p class="mb-0">Dölj högermenyn</p>
								</div>
								<div class="col">
									<toggle-button v-model="hideRightMenuByDefault" :labels="{ checked: 'Ja', unchecked: 'Nej' }"/>
								</div>
							</div>
						</div>
						<div class="form-group">
							<div class="row">
								<div class="col-lg-2 col-md-2 col-sm-12 col-xs-12 pr-0">
									<p class="mb-0">Dölj vänstermenyn</p>
								</div>
								<div class="col">
									<toggle-button v-model="hideLeftMenuByDefault" :labels="{ checked: 'Ja', unchecked: 'Nej' }"/>
								</div>
							</div>
						</div>
						<div class="form-group">
							<div class="row">
								<div class="col-lg-2 col-md-2 col-sm-12 col-xs-12 pr-0">
									<p class="mb-0">Filtrera bort trådar</p>
								</div>
								<div class="col">
									<div class="form-check form-check-inline" v-for="thread in kvpThreads" :key="thread.key">
                                        <input class="form-check-input" type="checkbox" :id="thread.key" :value="thread" v-model="excludedThreads">
                                        <label class="form-check-label" :for="thread.key">{{ thread.value }}</label>
                                    </div>
								</div>
							</div>
						</div>
						<results :items="results" class="mb-3" />
						<div class="form-group mb-0">
							<div class="row">
								<div class="col">
									<button type="button" class="btn btn-sm btn-success btn-block" :disabled="!canSave" v-on:click="save()">Spara</button>
								</div>
							</div>
						</div>
					</div>
				</div>
				<template v-else>
					<loader v-bind="{ background: '#699ED0' }" />
				</template>
			</div>
		</div>
	</div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component, Watch } from 'vue-property-decorator'
import { Getter, Mutation, Action, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('profile', Getter)
const ModuleAction = namespace('profile', Action)
const ModuleMutation = namespace('profile', Mutation)

const ForumModuleGetter = namespace('forum', Getter)
const ForumModuleAction = namespace('forum', Action)

import { PageViewModel, KeyValuePair, Result, ResultType } from '../../../model/common'
import { Filter } from '../../../model/profile'
import { Thread, ThreadType } from '../../../model/forum'

import { GET_USER, FETCH_USER, FETCH_KVP_USERS, CHANGE_FILTER } from '../../../modules/profile/types'
import { GET_THREADS, FETCH_THREADS } from '../../../modules/forum/types'

import Results from '../../../components/results.vue'

import { Typeahead } from 'uiv'

@Component({
	components: {
		Typeahead, Results
	}
})
export default class FilterComponent extends Vue {
	@State(state => state.vm) vm: PageViewModel
	@ModuleAction(FETCH_KVP_USERS) loadUsers: () => Promise<KeyValuePair[]>
	@ModuleAction(CHANGE_FILTER) changeFilter: (payload: { filter: Filter }) => Promise<Result[]>

	@ForumModuleGetter(GET_THREADS) threads: Thread[]
	@ForumModuleAction(FETCH_THREADS) loadThreads: () => Promise<void>

	users: KeyValuePair[] = []
	excludedUsers: KeyValuePair[] = []
	selectedUser: any = ''
	pageSize: number = 0
	hideLeftMenuByDefault: boolean = false
	hideRightMenuByDefault: boolean = false
	excludedThreads: KeyValuePair[] = []
	loading: boolean = true

	results: Result[] = []

	mounted() {
		let filter: Filter = this.vm.loggedInUser.filter
		this.pageSize = filter.postsPerPage
		this.excludedUsers = filter.excludedUsers
		this.hideLeftMenuByDefault = filter.hideLeftMenu
		this.hideRightMenuByDefault = filter.hideRightMenu
		this.excludedThreads = filter.excludedThreads
		
		this.loadThreads()

		this.loadUsers()
			.then((users: KeyValuePair[]) => {
				this.users = users.filter((kvp: KeyValuePair) => {
					return !this.excludedUsers.map((e: KeyValuePair) => e.key).includes(kvp.key)
				})
				this.loading = false
			})
	}

	get kvpThreads() {
		return this.threads.filter(t => t.type == ThreadType.Ongoing).map(t => { return {
				key: t.id,
				value: t.name
			}
		})
	}

	get canSave() {
		return this.pageSize > 0 && this.pageSize < 101
	}

	@Watch('selectedUser')
		onChange() {
			if (this.selectedUser && this.selectedUser.key) {
				this.excludedUsers.unshift(this.selectedUser)
				this.users = this.users.filter((kvp: KeyValuePair) => kvp.key !== this.selectedUser.key)
			}
		}

	removeUser(selected: KeyValuePair) {
		this.excludedUsers = this.excludedUsers.filter((kvp: KeyValuePair) => kvp.key !== selected.key)
	}

	save() {
		this.loading = true
		this.changeFilter({ filter: {
			userId: this.vm.loggedInUser.id,
			postsPerPage: this.pageSize,
			hideLeftMenu: this.hideLeftMenuByDefault,
			hideRightMenu: this.hideRightMenuByDefault,
			excludedUsers: this.excludedUsers,
			excludedThreads: this.excludedThreads
		}}).then((results: Result[]) => {
			this.results = results
			this.loading = false
		})
	}
}
</script>