<template>
    <nav class="navbar navbar-expand-md fixed-top navbar-dark bg-dark app-navbar">
		<button class="navbar-toggler navbar-toggler-right d-md-none" type="button" data-toggle="collapse" data-target="#navbarResponsive" aria-controls="navbarResponsive" aria-expanded="false" aria-label="Toggle navigation">
			<span class="navbar-toggler-icon"></span>
		</button>
		<div class="collapse navbar-collapse" id="navbarResponsive">
			<ul class="navbar-nav mr-auto">
				<li class="nav-item" :class="{ 'active': this.active('forum') }">
					<a class="nav-link" href="/forum">forum <span class="sr-only">(current)</span></a>
				</li>
				<li class="nav-item" :class="{ 'active': this.active('squad') }">
					<a class="nav-link" href="/squad">trupp</a>
				</li>
			</ul>
			<div class="form-inline float-right d-none d-md-flex">
				<input id="search_users" class="form-control" type="text" placeholder="sök efter en användare" data-action="grow" :disabled="loading" autocomplete="off">
				<typeahead v-model="selectedUser" target="#search_users" :data="users" item-key="value" force-select />
			</div>
			<ul id="#js-popoverContent" class="nav navbar-nav float-right mr-0 d-none d-md-flex">
				<li class="nav-item ml-2">
					<button class="btn btn-default navbar-btn navbar-btn-avatar" data-toggle="popover">
						<img class="rounded-circle" src="/uploads/avatars/generic/logo.png">
					</button>
				</li>
			</ul>
			<ul class="nav navbar-nav d-none" id="js-popoverContent">
				<li class="nav-item" :class="{ 'active': this.active('profile') }"><a class="nav-link" href="/profile">visa profil</a></li>
				<li class="nav-item"><a class="nav-link" href="/auth/logout">logga ut</a></li>
			</ul>
		</div>
	</nav>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component, Watch } from 'vue-property-decorator'
import { Getter, Action, Mutation, State, namespace } from 'vuex-class'

const ModuleAction = namespace('profile', Action)

import { FETCH_KVP_USERS } from '../../../../modules/profile/types'

import { ViewModel, KeyValuePair } from '../../../../model/common'

import { Typeahead } from 'uiv'

@Component({
    components: {
        Typeahead
    }
})
export default class Navbar extends Vue {
	@State(state => state.vm) vm: ViewModel
	@ModuleAction(FETCH_KVP_USERS) loadUsers: () => Promise<KeyValuePair[]>

	loading: boolean = true
	users: KeyValuePair[] = []

	selectedUser: any = ''

	@Watch('selectedUser')
		onChange() {
			console.log(this.selectedUser)
			if (this.selectedUser && this.selectedUser.key) {
				window.location.href = 'http://localhost:5000/profile/' + this.selectedUser.key
			}
		}

	mounted() {
		this.loadUsers().then((users: KeyValuePair[]) => {
			this.users = users
			this.loading = false
		})
	}

	active(page: string): boolean {
		return this.vm ? this.vm.page == page ? true : false : false
	}
}
</script>

<style lang="scss" scoped>
.dropdown {
	position: absolute;
	margin-top: 1rem;
}
</style>
