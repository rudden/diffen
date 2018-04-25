<template>
	<div class="container container__profile mt-3 mb-5">
		<template v-if="loading">
			<loader v-bind="{ background: '#699ED0' }" />
		</template>
		<template v-else>
			<div class="row" v-if="userIsLoggedInUser">
				<div class="col">
					<input id="pm-users" class="form-control" type="text" placeholder="sök på ett nick..">
					<typeahead v-model="selectedUser" target="#pm-users" :data="users" item-key="value" force-select />
				</div>
			</div>
			<div class="row mt-3">
				<div class="col">
					<span class="badge badge-secondary mr-2" v-for="convUser in conversationUsers" v-bind:key="convUser.key">{{ convUser.value }}</span>
				</div>
			</div>
			<template v-if="selectedUser && selectedUser.key || !userIsLoggedInUser">
				<div class="row" :class="{ 'mt-3' : userIsLoggedInUser }">
					<div class="col">
						<div class="form-group">
							<textarea class="form-control" v-model="newPmMessage" rows="2" placeholder="ditt pm"></textarea>
						</div>
						<div class="form-group mb-0">
							<div class="row">
								<div class="col">
									<button class="btn btn-sm btn-success btn-block" v-on:click="submit" v-bind:disabled="!canCreate">skicka</button>
								</div>
							</div>
						</div>
        				<results :items="results" :dismiss="dismiss" class="pt-3" />
					</div>
				</div>
			</template>
			<template v-if="pms.length > 0">
				<ul class="media-list media-list-conversation c-w-md mt-3 mb-0">
					<li class="media mb-4" v-for="pm in pms" v-bind:key="pm.id" :class="{ 'media-current-user': pm.from.id == vm.loggedInUser.id }">
						<template v-if="pm.from.id !== vm.loggedInUser.id">
							<img class="rounded-circle media-object ml-3" :src="pm.from.avatar">
						</template>
						<div class="media-body">
							<div class="media-body-text">
								<span>{{ pm.message }}</span>
							</div>
							<div class="media-footer">
								<small class="text-muted">
									<a :href="'/profile/' + pm.from.id">{{ pm.from.nickName }}</a> · {{ pm.since }}
								</small>
							</div>
						</div>
						<template v-if="pm.from.id == vm.loggedInUser.id">
							<img class="rounded-circle media-object ml-3" :src="pm.from.avatar">
						</template>
					</li>
				</ul>
			</template>
			<template v-if="pms.length == 0 && selectedUser && selectedUser.key">
				<div class="alert alert-warning mb-0 mt-3">
					hittade ingen konversation mellan dig och <strong>{{ selectedUser.value }}</strong>
				</div>
			</template>
		</template>
	</div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component, Watch } from 'vue-property-decorator'
import { Getter, Mutation, Action, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('profile', Getter)
const ModuleAction = namespace('profile', Action)
const ModuleMutation = namespace('profile', Mutation)

import { ProfileViewModel, KeyValuePair, Result, ResultType } from '../../../model/common'
import { PersonalMessage } from '../../../model/profile'
import { PersonalMessage as CrudPm } from '../../../model/profile/crud'

import { FETCH_KVP_USERS, FETCH_CONVERSATION_KVP_USERS, FETCH_PERSONAL_MESSAGES, CREATE_PM } from '../../../modules/profile/types'

import Results from '../../../components/results.vue'

import { Typeahead } from 'uiv'
import { Stretch as Loader } from 'vue-loading-spinner'

@Component({
	components: {
		Typeahead, Results, Loader
	}
})
export default class Pm extends Vue {
	@State(state => state.vm) vm: ProfileViewModel
	@ModuleAction(FETCH_KVP_USERS) loadUsers: () => Promise<KeyValuePair[]>
	@ModuleAction(FETCH_CONVERSATION_KVP_USERS) loadConversationUsers: (payload: { userId: string }) => Promise<KeyValuePair[]>
	@ModuleAction(FETCH_PERSONAL_MESSAGES) loadPms: (payload: { to?: string }) => Promise<PersonalMessage[]>
	@ModuleAction(CREATE_PM) create: (payload: { pm: CrudPm }) => Promise<Result[]>

	newPmMessage: string = ''
	selectedUser: any = ''
	loading: boolean = false
	results: Result[] = []
	
	pms: PersonalMessage[] = []
	users: KeyValuePair[] = []
	conversationUsers: KeyValuePair[] = []

	get canCreate() {
		return this.newPmMessage.length > 0
	}
	get userIsLoggedInUser() {
		return this.vm.selectedUserId ? this.vm.selectedUserId == this.vm.loggedInUser.id ? true : false : true
	}

	@Watch('selectedUser')
		onChange() {
			if (this.selectedUser && this.selectedUser.key) {
				this.loading = true
				this.loadPms({ to: this.selectedUser.key })
					.then((pms: PersonalMessage[]) => {
						this.pms = pms
						this.loading = false
					})
			}
		}

	mounted() {
		this.loadConversationUsers({ userId: this.vm.loggedInUser.id })
			.then((users: KeyValuePair[]) => this.conversationUsers = users)
		if (this.vm.selectedUserId && !this.userIsLoggedInUser) {
			this.loadPms({ to: this.vm.selectedUserId })
				.then((pms: PersonalMessage[]) => {
						this.pms = pms
						this.$forceUpdate()
					})
		} else {
			this.loadUsers()
				.then((users: KeyValuePair[]) => this.users = users)
		}
	}

	submit() {
		let toUserId: string = this.userIsLoggedInUser ? this.selectedUser.key : this.vm.selectedUserId
		this.create({ pm: { fromUserId: this.vm.loggedInUser.id, toUserId: toUserId, message: this.newPmMessage } })
			.then((results: Result[]) => {
				this.results = results
				this.newPmMessage = ''
				this.loadPms({ to: toUserId })
					.then((pms: PersonalMessage[]) => {
						this.pms = pms
						this.$forceUpdate()
					})
			})
	}
}
</script>