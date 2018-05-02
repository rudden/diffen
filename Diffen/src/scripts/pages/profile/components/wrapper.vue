<template>
	<div>
		<navbar />
		<div class="profile-header" style="background-image: url(/bg.jpg);" v-if="!loading">
			<user-component />
			<nav class="profile-header-nav">
				<ul class="nav nav-tabs justify-content-center">
					<li class="nav-item" v-for="nav in navs" v-bind:key="nav.id">
						<a class="nav-link" :class="{ 'active': nav.active }" v-on:click="toggle(nav.id)" href="#">{{ nav.text }}</a>
					</li>
				</ul>
			</nav>
		</div>
		<div class="container-my-4" v-if="!loading">
			<component v-bind:is="active" />
		</div>
	</div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Action, Mutation, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('profile', Getter)
const ModuleAction = namespace('profile', Action)
const ModuleMutation = namespace('profile', Mutation)

import { User } from '../../../model/profile/'
import { ProfileViewModel, NavItem } from '../../../model/common'
import { Component as VueComponent } from 'vue/types/options'

import { GET_USER, FETCH_USER, SET_USER } from '../../../modules/profile/types'

import Lineups from './lineups.vue'
import Pm from './pm.vue'
import Invites from './invites.vue'
import FilterComponent from './filter.vue'
import Posts from './posts.vue'
import UserComponent from './user.vue'

@Component({
	components: {
		Lineups, Pm, Invites, FilterComponent, Posts, UserComponent
	}
})
export default class Wrapper extends Vue {
	@State(state => state.vm) vm: ProfileViewModel
	@ModuleGetter(GET_USER) user: User
	@ModuleAction(FETCH_USER) loadUser: (payload: { id: string }) => Promise<void>
	@ModuleMutation(SET_USER) setUser: (user: User) => void

	loading: boolean = true
	navItems: NavItem[] = []

	mounted() {
		if (this.vm.selectedUserId) {
			this.loadUser({ id: this.vm.selectedUserId })
				.then(() => {
					this.setComponents()
					this.loading = false
				})
		} else {
			this.setUser(this.vm.loggedInUser)
			this.setComponents()
			this.loading = false
		}
	}

	get navs() {
		return this.navItems.filter((n: NavItem) => n.available)
	}
	get active() {
		return this.navItems.filter((c: NavItem) => c.active)[0].component
	}
	
	toggle(id: number) {
		for (let i = 0; i < this.navItems.length; i++) {
			if (this.navItems[i].id !== id)
				this.navItems[i].active = false
			else
				this.navItems[i].active = true
		}
	}

	setComponents() {
		this.navItems = [
			{
				id: 1,
				text: 'Pm',
				component: Pm,
				available: true,
				active: true
			},
			{
				id: 2,
				text: 'Startelvor',
				component: Lineups,
				available: true,
				active: false
			},
			{
				id: 3,
				text: 'Inbjudningar',
				component: Invites,
				available: this.user.id == this.vm.loggedInUser.id,
				active: false
			},
			{
				id: 4,
				text: 'Filter',
				component: FilterComponent,
				available: this.user.id == this.vm.loggedInUser.id,
				active: false
			},
			{
				id: 5,
				text: 'Inlägg',
				component: Posts,
				available: true,
				active: false
			}
		]
		this.loading = false
	}
}
</script>