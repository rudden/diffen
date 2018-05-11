<template>
	<div class="container container__profile mt-3 mb-5">
		<div class="card">
			<div class="card-header">Inbjudningar</div>
			<div class="card-body">
				<div class="row" v-if="!loading">
					<div class="col">
						<button class="btn btn-sm btn-block" v-on:click="inCreate = !inCreate" :class="{ 'btn-success': !inCreate, 'btn-danger': inCreate }">
							{{ inCreate ? 'Avbryt' : 'Ny inbjudan' }}
						</button>
					</div>
					<div class="col">
						<modal v-bind="{ attributes: { name: 'invites', scrollable: true }, header: 'Inbjudningar', button: { classes: 'btn btn-sm btn-primary btn-block', text: 'Visa inbjudningar' }, onOpen: load }">
							<template slot="body">
								<template v-if="filteredInvites.length > 0">
									<table class="table table-sm mb-0">
										<thead class="thead-dark">
											<tr>
												<th scope="col">Kod</th>
												<th scope="col" v-if="isAdmin">Från</th>
												<th scope="col">Skapad</th>
												<th scope="col">Använd</th>
											</tr>
										</thead>
										<tbody>
											<tr v-for="invite in filteredInvites">
												<td>{{ invite.uniqueCode }}</td>
												<td v-if="isAdmin">
													<a style="color: black" v-bind:href="'/profil/' + invite.invitedBy.id">{{ invite.invitedBy.nickName }}</a>
												</td>
												<td>{{ invite.inviteSent }}</td>
												<td style="text-align: center">
													<template v-if="invite.accountIsCreated">
														<span class="icon icon-check"></span>	
													</template>
												</td>
											</tr>
										</tbody>
									</table>
								</template>
								<template v-else>
									<div class="alert alert-warning mb-0">
										{{ isAdmin ? 'Hittade inga inbjudningar' : 'Du har inte genererat några inbjudningskoder' }}
									</div>
								</template>
							</template>
						</modal>
					</div>
				</div>
				<template v-else>
					<loader v-bind="{ background: '#699ED0' }" />
				</template>
				<div class="row" v-if="inCreate">
					<div class="col">
						<hr />
						<div class="form-group">
							<div class="row">
								<div class="col-lg-2 col-md-2 col-sm-12 col-xs-12 pr-0">
									<p class="mb-0">Antal inbjudningar</p>
								</div>
								<div class="col">
									<input type="number" v-model="newInvite.amount" class="form-control form-control-sm" min="1" max="10" />
								</div>
							</div>
						</div>
						<div class="form-group" v-if="generatedCodes.length > 0">
							<div class="card">
								<div class="card-header">
									Dina nya koder att dela med dig av
								</div>
								<div class="card-body pb-0">
									<ol class="pl-3">
										<li v-for="code in generatedCodes" :key="code">{{ code }}</li>
									</ol>
								</div>
							</div>
						</div>
						<div class="row">
							<div class="col">
								<button class="btn btn-success btn-block btn-sm" data-dismiss="modal" :disabled="newInvite.amount <= 0 && newInvite.amount > 10" v-on:click="send">Generera koder</button>
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

import { PageViewModel } from '../../../model/common'
import { Invite as CrudInvite } from '../../../model/profile/crud'
import { Invite } from '../../../model/profile'

import { FETCH_INVITES, CREATE_INVITE } from '../../../modules/profile/types'

import Modal from '../../../components/modal.vue'

@Component({
	components: {
		Modal
	}
})
export default class Invites extends Vue {
	@State(state => state.vm) vm: PageViewModel
	@ModuleAction(FETCH_INVITES) loadInvites: () => Promise<Invite[]>
	@ModuleAction(CREATE_INVITE) createInvite: (payload: { invite: CrudInvite }) => Promise<string[]>

	inCreate: boolean = false
	loading: boolean = false
	
	invites: Invite[] = []
	generatedCodes: string[] = []

	newInvite: CrudInvite = new CrudInvite()

	get isAdmin(): boolean {
		return this.vm.loggedInUser.inRoles.some((role: string) => role == 'Admin' || role == 'Scissor')
	}

	get filteredInvites() {
		return this.isAdmin ? this.invites : this.invites.filter((i: Invite) => i.invitedBy.id == this.vm.loggedInUser.id)
	}

	load() {
		this.loadInvites().then((invites: Invite[]) => this.invites = invites)
	}

	send() {
		this.loading = true
		this.newInvite.invitedByUserId = this.vm.loggedInUser.id
		this.createInvite({ invite: this.newInvite })
			.then((codes: string[]) => {
				this.generatedCodes = codes
				this.loading = false
			})
	}
}
</script>