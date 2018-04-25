<template>
	<div class="col-lg-3">
		<filter-component />
		<polls-component :type-of-polls="'active'" :is-small="true" class="mb-4" />
		<chronicles-component :is-small="true" class="mb-4" />
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
import { State, namespace } from 'vuex-class'

import FilterComponent from './filter.vue'
import PollsComponent from '../../../components/other/polls.vue'
import ChroniclesComponent from '../../../components/other/chronicles.vue'

@Component({
	components: {
		FilterComponent, PollsComponent, ChroniclesComponent
	}
})
export default class RightSideBar extends Vue {
	currentYear: number = (new Date()).getFullYear()
}
</script>	