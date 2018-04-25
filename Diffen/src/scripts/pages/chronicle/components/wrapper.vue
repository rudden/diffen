<template>
	<div class="container pt-4 pb-5 chronicle">
		<div class="row" v-if="!loading">
            <div class="col">
    			<component :is="active.component" v-bind="active.attributes ? active.attributes : {}" />
            </div>
		</div>
	</div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { State, namespace } from 'vuex-class'

import { ChronicleViewModel, Result, ResultType, NavItem } from '../../../model/common'

import Chronicle from '../../../components/other/chronicle.vue'
import Chronicles from '../../../components/other/chronicles.vue'
import NewChronicle from '../../../components/other/new-chronicle.vue'
import { Stretch as Loader } from 'vue-loading-spinner'

import { Component as VueComponent } from 'vue/types/options'

@Component({
	components: {
        Loader, Chronicles, NewChronicle
	}
})
export default class Wrapper extends Vue {
    @State(state => state.vm) vm: ChronicleViewModel

    loading: boolean = true
	navItems: NavItem[] = []

    mounted() {
        this.navItems = [
			{
				id: 1,
				component: Chronicles,
				active: !this.singleChronicleSelected && !this.createNewChronicleSelected
			},
			{
				id: 2,
				component: Chronicle,
				active: this.singleChronicleSelected && !this.createNewChronicleSelected
			},
			{
				id: 3,
				component: NewChronicle,
				attributes: {
					selectedChronicleSlug: this.vm.selectedChronicleSlug
				},
				active: this.createNewChronicleSelected || (this.singleChronicleSelected && this.createNewChronicleSelected)
			}
		]
		this.loading = false
    }

	get active() {
		return this.navItems.filter((c: NavItem) => c.active)[0]
	}

	get createNewChronicleSelected() {
		return this.vm.inCreate
	}

    get singleChronicleSelected() {
        return this.vm.selectedChronicleSlug ? true : false
    }
}
</script>

<style lang="scss" scoped>
a {
    cursor: pointer;
}
</style>
