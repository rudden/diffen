<template>
	<div>
		<div class="profile-header" style="background-image: url(background.jpg);">
			<div class="container">
				<div class="container-inner">
					<img class="rounded-circle media-object" :src="user.avatar">
					<h3 class="profile-header-user">{{ user.nickName }}</h3>
					<p class="profile-header-bio">{{ user.bio }}</p>
				</div>
			</div>
			<nav class="profile-header-nav">
				<ul class="nav nav-tabs justify-content-center">
					<li class="nav-item" v-for="item in navItems" v-bind:key="item.id">
						<a class="nav-link" :class="{ 'active': item.active }" v-on:click="toggle(item.id)" href="#">{{ item.text }}</a>
					</li>
				</ul>
			</nav>
		</div>
		<div class="container-my-4">
			<component v-bind:is="active" />
		</div>
	</div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Mutation, Action, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('profile', Getter)
const ModuleAction = namespace('profile', Action)
const ModuleMutation = namespace('profile', Mutation)

import { ViewModel } from '../../../model/common'

import { GET_USER, FETCH_USER } from '../../../modules/profile/types'

import Lineups from './lineups.vue'
import Pm from './pm.vue'
import Invites from './invites.vue'
import FilterComponent from './filter.vue'
import Posts from './posts.vue'

import { Component as VueComponent } from 'vue/types/options'

interface NavItem {
	id: number,
	text: string,
	component: VueComponent,
	active: boolean
}

@Component({
	components: {
		Lineups, Pm, Invites, FilterComponent, Posts
	}
})
export default class Wrapper extends Vue {
	@State(state => state.vm) vm: ViewModel

	navItems: NavItem[] = [
		{ id: 1, text: 'startelvor', component: Lineups, active: true },
		{ id: 2, text: 'personliga meddelanden', component: Pm, active: false },
		{ id: 3, text: 'inbjudningar', component: Invites, active: false },
		{ id: 4, text: 'forumfilter', component: FilterComponent, active: false },
		{ id: 5, text: 'inlägg', component: Posts, active: false }
	]

	get user() {
		return this.vm.loggedInUser
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
}
</script>

<style lang="scss" scoped>
.nav-link.active {
	font-weight: bold;
}
</style>
