<template>
	<div>
		<navbar />
		<div class="container pt-4 pb-5">
			<div class="row" v-if="!loading">
				<div class="col">
					<component :is="active.component" v-bind="active.attributes ? active.attributes : {}" />
				</div>
			</div>
		</div>
	</div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { State, namespace } from 'vuex-class'

import { PollViewModel, NavItem } from '../../../model/common'

import Poll from '../../../components/other/poll.vue'
import Polls from '../../../components/other/polls.vue'

@Component({
	components: {
        Polls, Poll
	}
})
export default class Wrapper extends Vue {
    @State(state => state.vm) vm: PollViewModel

    loading: boolean = true
	navItems: NavItem[] = []

    mounted() {
        this.navItems = [
			{
				id: 1,
				component: Poll,
				attributes: {
					slug: this.vm.selectedPollSlug
				},
				active: this.singlePollSelected
			},
			{
				id: 2,
				component: Polls,
				attributes: {
					openInModal: false
				},
				active: !this.singlePollSelected
			},
		]
		this.loading = false
    }

	get active() {
		return this.navItems.filter((c: NavItem) => c.active)[0]
	}
    get singlePollSelected() {
        return this.vm.selectedPollSlug ? true : false
    }
}
</script>