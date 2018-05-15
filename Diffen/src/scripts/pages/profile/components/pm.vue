<template>
	<div class="container container__profile mt-3 mb-5">
		<div v-show="loading">
			<loader v-bind="{ background: '#699ED0' }" />
		</div>
		<div v-show="!loading">
			<div class="row" v-if="userIsLoggedInUser">
				<div class="col">
					<input id="pm-users" class="form-control" type="text" placeholder="Sök på ett nick.." autocomplete="off">
					<typeahead v-model="selectedUser" target="#pm-users" :data="users" item-key="nickName" force-select />
				</div>
			</div>
			<div class="row mt-3" v-if="userIsLoggedInUser && conversations.length > 0">
				<div class="col">
					<p class="mb-0">Användare som du har en pågående konversation med</p>
					<a href="#" v-on:click="selectedUser = conversation.user" v-for="conversation in conversations" :key="conversation.user.id">
						<span class="badge badge-secondary mr-2 p-1">
							{{ conversation.user.nickName }} 
							<span class="badge badge-success ml-1" v-if="conversation.numberOfUnReadMessages > 0">{{ conversation.numberOfUnReadMessages }}</span>
						</span>
					</a>
					<results :items="loadResults" class="pt-3" />
				</div>
			</div>
			<template v-if="loadResults.length == 0">
				<template v-if="selectedUser && selectedUser.id || !userIsLoggedInUser">
					<div class="row" :class="{ 'mt-3' : userIsLoggedInUser }">
						<div class="col">
							<div class="form-group">
								<textarea class="form-control" v-model="newPmMessage" rows="8" placeholder="Ditt pm"></textarea>
							</div>
							<div class="form-group mb-0">
								<div class="row">
									<div class="col">
										<button class="btn btn-sm btn-success btn-block" v-on:click="submit" :disabled="!canCreate">Skicka</button>
									</div>
								</div>
							</div>
							<results :items="results" class="pt-3" />
						</div>
					</div>
				</template>
				<template v-if="pms.length > 0">
					<ul class="media-list media-list-conversation c-w-md mt-3 mb-0">
						<li class="media mb-4" v-for="pm in pms" :key="pm.id" :class="{ 'media-current-user': pm.from.id == vm.loggedInUser.id }">
							<template v-if="pm.from.id !== vm.loggedInUser.id">
								<img class="rounded-circle media-object ml-3" :src="pm.from.avatar">
							</template>
							<div class="media-body">
								<div class="media-body-text">
									<span>{{ pm.message }}</span>
								</div>
								<div class="media-footer">
									<small class="text-muted">
										<a :href="'/profil/' + pm.from.id">{{ pm.from.nickName }}</a> · {{ pm.since }}
									</small>
								</div>
							</div>
							<template v-if="pm.from.id == vm.loggedInUser.id">
								<img class="rounded-circle media-object ml-3" :src="pm.from.avatar">
							</template>
						</li>
					</ul>
				</template>
				<template v-if="pms.length == 0 && selectedUser && selectedUser.id">
					<div class="alert alert-warning mb-0 mt-3">
						Hittade ingen konversation mellan dig och <strong>{{ selectedUser.nickName }}</strong>
					</div>
				</template>
			</template>
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

import { ProfileViewModel, KeyValuePair, Result, ResultType } from '../../../model/common'
import { PersonalMessage, Conversation } from '../../../model/profile'
import { PersonalMessage as CrudPm } from '../../../model/profile/crud'

import { FETCH_KVP_USERS, FETCH_CONVERSATION_KVP_USERS, FETCH_PERSONAL_MESSAGES, CREATE_PM } from '../../../modules/profile/types'

import Results from '../../../components/results.vue'

import { Typeahead } from 'uiv'

@Component({
	components: {
		Typeahead, Results
	}
})
export default class Pm extends Vue {
	@State(state => state.vm) vm: ProfileViewModel
	@ModuleAction(FETCH_KVP_USERS) loadUsers: () => Promise<KeyValuePair[]>
	@ModuleAction(FETCH_CONVERSATION_KVP_USERS) loadConversationUsers: (payload: { userId: string }) => Promise<Conversation[]>
	@ModuleAction(FETCH_PERSONAL_MESSAGES) loadPms: (payload: { to?: string }) => Promise<PersonalMessage[]>
	@ModuleAction(CREATE_PM) create: (payload: { pm: CrudPm }) => Promise<Result[]>

	newPmMessage: string = ''
	selectedUser: any = ''
	loading: boolean = false
	results: Result[] = []

	loadResults: Result[] = []
	
	pms: PersonalMessage[] = []
	users: KeyValuePair[] = []
	conversations: Conversation[] = []

	get canCreate() {
		return this.newPmMessage.length > 0
	}
	get userIsLoggedInUser() {
		return this.vm.selectedUserId ? this.vm.selectedUserId == this.vm.loggedInUser.id ? true : false : true
	}

	@Watch('selectedUser')
		onChange() {
			if (this.selectedUser && this.selectedUser.id) {
				this.loading = true
				this.loadPms({ to: this.selectedUser.id })
					.then((pms: PersonalMessage[]) => {
						this.pms = pms
						let conversation = this.conversations.filter((c: Conversation) => c.user.id == this.selectedUser.id)[0]
						if (conversation) {
							conversation.numberOfUnReadMessages = 0
							let index = this.conversations.indexOf(conversation)
							if (index !== -1) {
								this.conversations[index] = conversation
							}
						}
						this.loading = false
					}).catch(() => this.setLoadResults(this.selectedUser.nickName))
			}
		}

	mounted() {
		this.loadConversationUsers({ userId: this.vm.loggedInUser.id })
			.then((conversations: Conversation[]) => this.conversations = conversations)

		if (this.vm.selectedUserId && !this.userIsLoggedInUser) {
			this.loadPms({ to: this.vm.selectedUserId })
				.then((pms: PersonalMessage[]) => {
						this.pms = pms
						this.$forceUpdate()
					}).catch(() => this.setLoadResults())
		} else {
			this.loadUsers()
				.then((users: KeyValuePair[]) => this.users = users)
		}
	}

	submit() {
		let toUserId: string = this.userIsLoggedInUser ? this.selectedUser.id : this.vm.selectedUserId
		this.create({ pm: { fromUserId: this.vm.loggedInUser.id, toUserId: toUserId, message: this.newPmMessage } })
			.then((results: Result[]) => {
				this.results = results
				this.newPmMessage = ''
				if (!this.conversations.map((conversation: Conversation) => conversation.user.id).includes(this.selectedUser.id)) {
					this.conversations.push({
						user: {
							id: this.selectedUser.id,
							nickName: this.selectedUser.nickName
						},
						numberOfUnReadMessages: 0
					})
				}
				this.loadPms({ to: toUserId })
					.then((pms: PersonalMessage[]) => {
						this.pms = pms
						this.$forceUpdate()
					})
			})
	}

	setLoadResults(withUserNickName: string = '') {
		this.loadResults = []
		this.loadResults.push({ type: ResultType.Failure, message: `kunde inte ladda pm med ${withUserNickName ? withUserNickName : ''}` })
		this.loading = false
	}
}
</script>