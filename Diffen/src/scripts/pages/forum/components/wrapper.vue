<template>
	<div class="container pt-4 pb-5">
		<div class="row">
			<left-sidebar />
			<middle />
			<right-sidebar />
		</div>
	</div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'

import LeftSidebar from './left-sidebar.vue'
import Middle from './middle.vue'
import RightSidebar from './right-sidebar.vue'

@Component({
	components: {
		LeftSidebar,
		Middle,
		RightSidebar
	}
})
export default class Wrapper extends Vue {}
</script>