<template>
    <div class="row">
        <left-sidebar />
        <middle />
        <right-sidebar />
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
export default class Main extends Vue {}
</script>