<template>
	<div>
		<navbar />
		<div class="container pt-3 pb-4" v-if="!loading">
			<component :is="active.component" v-bind="active.attributes ? active.attributes : {}" />
		</div>
	</div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { State, namespace } from 'vuex-class'

import MainComponent from './main.vue'
import FullConversation from '../../../components/post/full-conversation.vue'

import { ForumViewModel, NavItem } from '../../../model/common/'

@Component({
	components: {
		MainComponent,
		FullConversation
	}
})
export default class Wrapper extends Vue {
	@State(state => state.vm) vm: ForumViewModel
	
	navItems: NavItem[] = []

	loading: boolean = true
	
	get active() {
		return this.navItems.filter((c: NavItem) => c.active)[0]
	}
	
	mounted() {
		this.navItems = [
			{
				id: 1,
				component: MainComponent,
				active: this.vm.selectedPostId == 0 ? true : false
			},
			{
				id: 2,
				component: FullConversation,
				attributes: {
					selectedPostId: this.vm.selectedPostId
				},
				active: this.vm.selectedPostId > 0 ? true : false
			}
		]
		this.loading = false
	}
}
</script>