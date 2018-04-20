<template>
	<div class="container container__profile mt-3 mb-5">
		<div class="card">
			<div class="card-header">inbjudningar</div>
			<div class="card-body">
				<div class="row" v-if="!loading">
					<div class="col">
						<button class="btn btn-sm btn-block" v-on:click="inCreate = !inCreate" :class="{ 'btn-success': !inCreate, 'btn-danger': inCreate }">
							{{ inCreate ? 'avbryt' : 'ny inbjudan' }}
						</button>
					</div>
					<template v-if="isAdmin">
						<div class="col">
							<modal v-bind="{ header: 'inbjudningar', id: 'invites' }">
								<template slot="btn">
									<button data-toggle="modal" data-target="#invites" class="btn btn-sm btn-primary btn-block" v-on:click="load">visa inbjudningar</button>
								</template>
								<template slot="body">
									<template v-if="invites.length > 0">
										<table class="table table-sm mb-0">
											<thead class="thead-dark">
												<tr>
													<th scope="col">email</th>
													<th scope="col">har skapat konto</th>
													<th scope="col">inbjudan skickad</th>
													<th scope="col">kontot skapat</th>
													<th scope="col">inbjuden av</th>
												</tr>
											</thead>
											<tbody>
												<tr v-for="invite in invites">
													<td>{{ invite.email }}</td>
													<td style="text-align: center">
														<template v-if="invite.accountIsCreated">
															<span class="icon icon-check"></span>	
														</template>
													</td>
													<td>{{ invite.inviteSent }}</td>
													<td>{{ invite.accountCreated }}</td>
													<td>
														<a style="color: black" v-bind:href="'/profile/' + invite.invitedBy.id">{{ invite.invitedBy.nickName }}</a>
													</td>
												</tr>
											</tbody>
										</table>
									</template>
								</template>
							</modal>
						</div>
					</template>
				</div>
				<template v-else>
					<loader v-bind="{ background: '#699ED0' }" />
				</template>
				<div class="row" v-if="inCreate">
					<div class="col">
						<hr />
						<div class="form-group">
							<input type="email" class="form-control" v-model="newInvite.email" placeholder="personen@mail.com" required />
						</div>
						<results :items="results" :dismiss="dismiss" class="mb-3" />
						<div class="row">
							<div class="col">
								<button class="btn btn-success btn-block btn-sm" data-dismiss="modal" :disabled="!canSend" v-on:click="send">skicka</button>
							</div>
						</div>
					</div>
				</div>
			</div>
		</div>
	</div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Action, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('profile', Getter)
const ModuleAction = namespace('profile', Action)

import { ViewModel, Result, ResultType } from '../../../model/common'
import { Invite as CrudInvite } from '../../../model/profile/crud'
import { Invite } from '../../../model/profile'

import { FETCH_INVITES, CREATE_INVITE } from '../../../modules/profile/types'

import Modal from '../../../components/modal.vue'
import Results from '../../../components/results.vue'
import { Stretch as Loader } from 'vue-loading-spinner'

@Component({
	components: {
		Loader, Modal, Results
	}
})
export default class Invites extends Vue {
	@State(state => state.vm) vm: ViewModel
	@ModuleAction(FETCH_INVITES) loadInvites: () => Promise<Invite[]>
	@ModuleAction(CREATE_INVITE) createInvite: (payload: { invite: CrudInvite }) => Promise<Result[]>

	inCreate: boolean = false
	loading: boolean = false
	
	results: Result[] = []
	invites: Invite[] = []

	newInvite: CrudInvite = new CrudInvite()

	get isAdmin(): boolean {
		return this.vm.loggedInUser.inRoles.some((role: string) => role == 'Admin' || role == 'Sax')
	}
	get canSend(): boolean {
    	var regex = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
	    return regex.test(String(this.newInvite.email).toLowerCase())
	}

	load() {
		this.loadInvites().then((invites: Invite[]) => this.invites = invites)
	}

	send() {
		this.loading = true
		this.newInvite.invitedByUserId = this.vm.loggedInUser.id
		this.createInvite({ invite: this.newInvite })
			.then((results: Result[]) => {
				this.results = results
				this.loading = false
			})
	}

	dismiss(type: ResultType) {
		this.results = this.results.filter((r: Result) => r.type != type)
	}
}
</script>