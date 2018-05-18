<template>
	<div class="col-lg-3" v-show="showRightSideBar">
		<filter-component />
		<!-- <polls-component :type-of-polls="'active'" :is-small="true" class="mb-4" />
		<chronicles-component :is-small="true" :amount-of-chronicles="5" class="mb-4" /> -->
		<rss-feed :url="'https://www.jarnkaminerna.se/feed/'" :feed-name="'Senaste från Järnkaminerna'" />
		<rss-feed :url="'https://diftv.solidtango.com/feed/'" :feed-name="'Senaste från DIFTV'" />
		<div class="card card-link-list">
			<div class="card-body">
				© {{ currentYear }} Blåränderna
			</div>
		</div>
	</div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Mutation, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('forum', Getter)
const ModuleMutation = namespace('forum', Mutation)

import { GET_SHOW_RIGHT_SIDEBAR, SET_SHOW_RIGHT_SIDEBAR } from '../../../modules/forum/types'

import { PageViewModel } from '../../../model/common'

import FilterComponent from './filter.vue'
import PollsComponent from '../../../components/other/polls.vue'
import ChroniclesComponent from '../../../components/other/chronicles.vue'
import RssFeed from '../../../components/rss.vue'

@Component({
	components: {
		FilterComponent, PollsComponent, ChroniclesComponent, RssFeed
	}
})
export default class RightSideBar extends Vue {
    @State(state => state.vm) vm: PageViewModel
	@ModuleGetter(GET_SHOW_RIGHT_SIDEBAR) showRightSideBar: boolean
    @ModuleMutation(SET_SHOW_RIGHT_SIDEBAR) setShowRightSideBar: (payload: { value: boolean }) => void
	
	currentYear: number = (new Date()).getFullYear()

	mounted() {
        this.setShowRightSideBar({ value: !this.vm.loggedInUser.filter.hideRightMenu })
	}
}
</script>