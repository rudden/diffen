<template>
	<div class="row">
		<left-sidebar />
		<posts />
		<right-sidebar />
	</div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'

import LeftSidebar from './left-sidebar.vue'
import Posts from './posts.vue'
import RightSidebar from './right-sidebar.vue'

@Component({
	components: {
		LeftSidebar,
		Posts,
		RightSidebar
	}
})
export default class Wrapper extends Vue {}
</script>