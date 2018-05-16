<template>
    <ul class="timeline">
        <li v-for="(title, index) in titles" :class="{ 'timeline-inverted': index % 2 }">
            <div class="timeline-badge primary"><span class="icon icon-trophy"></span></div>
            <div class="timeline-panel">
                <div class="timeline-heading">
                    <h4 class="timeline-title">{{ getTypeOfTitle(title.type) }}</h4>
                    <p :class="{ 'mb-0': !title.description }"><small class="text-muted"><span class="icon icon-clock"></span> {{ title.year }}</small></p>
                </div>
                <div class="timeline-body" v-if="title.description">
                    <p>{{ title.description }}</p>
                </div>
            </div>
        </li>
    </ul>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Action, State, namespace } from 'vuex-class'

const ModuleAction = namespace('squad', Action)

import { Title, TitleType } from '../../../model/squad'
import { PageViewModel } from '../../../model/common'

import { FETCH_TITLES } from '../../../modules/squad/types'

import Modal from '../../../components/modal.vue'

@Component({
	components: {
		Modal
	}
})
export default class Titles extends Vue {
	@State(state => state.vm) vm: PageViewModel
    @ModuleAction(FETCH_TITLES) loadTitles: () => Promise<Title[]>

    loading: boolean = true
    titles: Title[] = []

	mounted() {
		this.loadTitles().then((titles: Title[]) => {
            this.titles = titles
            this.loading = false
        })
	}

	get loggedInUserIsAdmin(): boolean {
        return this.vm.loggedInUser.inRoles.some(role => role == 'Admin')
    }

    getTypeOfTitle(type: TitleType): string {
        switch (type) {
            case TitleType.Cup:
                return 'Cup-mästare'
            case TitleType.League:
                return 'Svenska Mästare'
            default:
                return ''
        }
    }
}
</script>

<style lang="scss" scoped>

</style>