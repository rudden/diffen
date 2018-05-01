<template>
	<div class="col-lg-3" v-show="showRightSideBar">
		<filter-component />
		<polls-component :type-of-polls="'active'" :is-small="true" class="mb-4" />
		<chronicles-component :is-small="true" :amount-of-chronicles="5" class="mb-4" />
		<div class="card card-link-list">
			<div class="card-body">
				© {{ currentYear }} Diffen
			</div>
		</div>
	</div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('forum', Getter)

import { GET_SHOW_RIGHT_SIDEBAR } from '../../../modules/forum/types'

import FilterComponent from './filter.vue'
import PollsComponent from '../../../components/other/polls.vue'
import ChroniclesComponent from '../../../components/other/chronicles.vue'

@Component({
	components: {
		FilterComponent, PollsComponent, ChroniclesComponent
	}
})
export default class RightSideBar extends Vue {
	@ModuleGetter(GET_SHOW_RIGHT_SIDEBAR) showRightSideBar: boolean
	
	currentYear: number = (new Date()).getFullYear()
}
</script>