<template>
    <nav class="navbar navbar-expand-md fixed-top navbar-dark bg-dark app-navbar">
		<button class="navbar-toggler navbar-toggler-right d-md-none" type="button" data-toggle="collapse" data-target="#navbarResponsive" aria-controls="navbarResponsive" aria-expanded="false" aria-label="Toggle navigation">
			<span class="navbar-toggler-icon"></span>
		</button>
		<div class="collapse navbar-collapse" id="navbarResponsive">
			<ul class="navbar-nav mr-auto">
				<!-- <li class="nav-item" :class="{ 'active': this.active('home') }">
					<a class="nav-link" href="/">Hem <span class="sr-only">(current)</span></a>
				</li> -->
				<li class="nav-item" :class="{ 'active': this.active('forum') }">
					<a class="nav-link" href="/forum">Forum</a>
				</li>
				<li class="nav-item" :class="{ 'active': this.active('squad') }">
					<a class="nav-link" href="/trupp">Trupp</a>
				</li>
				<li class="nav-item" :class="{ 'active': this.active('game') }">
					<a class="nav-link" href="/matcher">Matcher</a>
				</li>
				<li class="nav-item" :class="{ 'active': this.active('chronicle') }">
					<a class="nav-link" href="/kronika">Krönikor</a>
				</li>
				<li class="nav-item" :class="{ 'active': this.active('poll') }">
					<a class="nav-link" href="/omrostning">Omröstningar</a>
				</li>
				<li class="nav-item" :class="{ 'active': this.active('region') }">
					<a class="nav-link" href="/omrade">Områden</a>
				</li>
				<li class="nav-item" :class="{ 'active': this.active('aboutdif') }">
					<a class="nav-link" href="/om-dif">Titlar</a>
				</li>
				<li class="navbar-divider small-device"></li>
				<li class="nav-item small-device" :class="{ 'active': this.active('profile') }">
					<a class="nav-link" href="/profil">
						Profil
						<span class="ml-1" v-if="vm.loggedInUser.numberOfUnReadPersonalMessages > 0">
							({{ vm.loggedInUser.numberOfUnReadPersonalMessages }} nya pm)
						</span>
					</a>
				</li>
			</ul>
			<div class="form-inline mt-0">
				<input id="search_users" class="form-control" type="text" placeholder="Sök efter en användare" data-action="grow" :disabled="loading" autocomplete="off">
				<typeahead v-model="selectedUser" target="#search_users" :data="users" item-key="value" force-select />
				<a href="/auth/logout" class="btn btn-outline-primary my-2 my-sm-0 ml-3 small-device">Logga ut</a>
			</div>
			<ul id="#js-popoverContent" class="nav navbar-nav float-right mr-0 d-none d-md-flex">
				<li class="nav-item" v-if="vm.loggedInUser.numberOfUnReadPersonalMessages > 0">
					<a class="app-notifications nav-link" href="/profil">
						<span class="icon icon-bell" v-tooltip.left="vm.loggedInUser.numberOfUnReadPersonalMessages + ' nya pm'"></span>
					</a>
				</li>
				<li class="nav-item ml-2">
					<button class="btn btn-default navbar-btn navbar-btn-avatar" data-toggle="popover">
						<img class="rounded-circle" :src="vm.loggedInUser.avatar">
					</button>
				</li>
			</ul>
			<ul class="nav navbar-nav d-none" id="js-popoverContent">
				<li class="nav-item"><a href="/profil" class="nav-link">Profil</a></li>
				<li class="nav-item"><a class="nav-link" href="/auth/logout">Logga ut</a></li>
			</ul>
		</div>
	</nav>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component, Watch } from 'vue-property-decorator'
import { Getter, Action, Mutation, State, namespace } from 'vuex-class'

const ModuleAction = namespace('profile', Action)

import { FETCH_KVP_USERS } from '../modules/profile/types'

import { ProfileViewModel, PageViewModel, KeyValuePair } from '../model/common'

import { Typeahead } from 'uiv'

@Component({
    components: {
        Typeahead
    }
})
export default class Navbar extends Vue {
	@State(state => state.vm) vm: PageViewModel
	@ModuleAction(FETCH_KVP_USERS) loadUsers: () => Promise<KeyValuePair[]>

	loading: boolean = true
	users: KeyValuePair[] = []

	selectedUser: any = ''

	private baseUrl: string

	@Watch('selectedUser')
		onChange() {
			if (this.selectedUser && this.selectedUser.key) {
				let newUrl: string = `${this.baseUrl}profil/${this.selectedUser.key}`
				if (window.location.href == newUrl)
					return
				this.redirectTo(newUrl)
			}
		}

	mounted() {
		var getUrl = window.location
		this.baseUrl = getUrl .protocol + '//' + getUrl.host + '/' + getUrl.pathname.split('/')[0]
		this.loadUsers()
			.then((users: KeyValuePair[]) => {
				this.users = users
				this.loading = false
				if (this.vm.page == 'profile') {
					let profileVm = this.vm as ProfileViewModel
					if (profileVm.selectedUserId) {
						this.selectedUser = this.users.filter((kvp: KeyValuePair) => kvp.key == profileVm.selectedUserId)[0]
					}
				}
			})
	}

	active(page: string): boolean {
		return this.vm ? this.vm.page == page ? true : false : false
	}

	redirectTo(href: string): void {
		window.location.href = href
	}
}
</script>

<style lang="scss" scoped>
.dropdown {
	position: absolute;
	margin-top: 1rem;
}
</style>
