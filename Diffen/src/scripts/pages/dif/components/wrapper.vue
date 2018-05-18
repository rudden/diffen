<template>
	<div>
		<navbar />
		<div class="container pt-4 pb-5">
			<div class="row" v-if="!loading">
                <!-- <div class="col-lg-3">
                    <div class="list-group mb-4">
                        <a href="#" v-on:click="toggle(item.id)" class="list-group-item list-group-item-action d-flex justify-content-between" v-for="item in navItems" :key="item.id" :class="{ 'active': item.id == active.id }">
                            <span>{{ item.text }}</span>
                            <span class="icon icon-chevron-thin-right"></span>
                        </a>
                    </div>
                </div>
                <div class="col-lg-9">
					<component :is="active.component" v-bind="active.attributes ? active.attributes : {}" />
                </div> -->
                <div class="col">
                    <titles />
                </div>
    		</div>
		</div>
	</div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Action, State, namespace } from 'vuex-class'

import { PageViewModel, NavItem } from '../../../model/common'

import { } from '../../../modules/squad/types'

import Titles from './titles.vue'
import HistoryComponent from './history.vue'

@Component({
	components: {
		Titles, HistoryComponent
	}
})
export default class Wrapper extends Vue {
    @State(state => state.vm) vm: PageViewModel
    
    loading: boolean = true

    navItems: NavItem[] = []

    mounted() {
        this.navItems = [
			{
				id: 1,
                component: Titles,
                text: 'Titlar',
				active: true
			},
			{
				id: 2,
                component: HistoryComponent,
                text: 'Historia',
				active: false
			},
		]
		this.loading = false
    }

	get loggedInUserIsAdmin(): boolean {
        return this.vm.loggedInUser.inRoles.some(role => role == 'Admin')
    }
	get active() {
		return this.navItems.filter((c: NavItem) => c.active)[0]
    }
    
    toggle(id: number) {
		for (let i = 0; i < this.navItems.length; i++) {
			if (this.navItems[i].id !== id)
				this.navItems[i].active = false
			else
				this.navItems[i].active = true
		}
	}
}
</script>