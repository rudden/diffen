<template>
	<div class="col-lg-3">
		<div class="alert alert-warning alert-dismissible d-none d-lg-block" role="alert">
			<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>
			<a class="alert-link" href="/profile">spana in din profil!</a> du kanske har fått ett nytt pm.
		</div>
		<filter-component />
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

import FilterComponent from './filter.vue'

@Component({
	components: {
		FilterComponent
	}
})
export default class RightSideBar extends Vue {
	currentYear: number = (new Date()).getFullYear()
}
</script>	