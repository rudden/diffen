<template>
    <ul class="list-group media-list media-list-stream">
        <li class="list-group-item" :class="{ 'p-3': isSmall, 'p-4': !isSmall }" v-if="loggedInUserIsAuthor || !isSmall">
            <a href="/chronicle/new" class="btn btn-sm btn-primary" :class="{ 'float-right': !isSmall, 'btn-block': isSmall }" v-if="loggedInUserIsAuthor">ny krönika</a>
            <h4 class="mb-0" v-if="!isSmall">krönikor</h4>
        </li>
        <li class="media list-group-item" :class="{ 'p-3': isSmall, 'p-4': !isSmall }" v-show="loading">
            <loader v-bind="{ background: '#699ED0' }" />
        </li>
        <div v-show="!loading">
            <template v-if="chronicles.length > 0">
                <li class="list-group-item media"  :class="{ 'p-3': isSmall, 'p-4': !isSmall }" v-for="chronicle in chronicles" :key="chronicle.id">
                    <span class="icon icon-pencil text-muted mr-2" v-if="!isSmall"></span>
                    <div class="media-body">
                        <small class="text-muted float-right">{{ chronicle.created }}</small>
                        <div class="media-heading">
                            <a :href="`/chronicle/${chronicle.slug}`">
                                <template v-if="isSmall">
                                    <small><strong>{{ chronicle.title }}</strong></small>
                                </template>
                                <template v-else>
                                    <strong>{{ chronicle.title }}</strong>
                                </template>
                            </a>
                        </div>
                    </div>
                </li>
            </template>
            <template v-else>
                <li class="list-group-item media" :class="{ 'p-3': isSmall, 'p-4': !isSmall }">
                    <div class="media-body">
                        <div class="alert alert-warning mb-0">hittade inga krönikor</div>
                    </div>
                </li>
            </template>
        </div>
    </ul>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Action, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('other', Getter)
const ModuleAction = namespace('other', Action)

import { PageViewModel } from '../../model/common'
import { Chronicle } from '../../model/other'

import { GET_CHRONICLES, FETCH_CHRONICLES } from '../../modules/other/types'

import Modal from '../modal.vue'
import { Stretch as Loader } from 'vue-loading-spinner'

@Component({
    props: {
        isSmall: {
            type: Boolean,
            default: false
        }
    },
	components: {
        Loader, Modal
	}
})
export default class Chronicles extends Vue {
    @State(state => state.vm) vm: PageViewModel
    @ModuleGetter(GET_CHRONICLES) chronicles: Chronicle[]
    @ModuleAction(FETCH_CHRONICLES) loadChronicles: () => Promise<void>

    isSmall: boolean

    loading: boolean = true

	mounted() {
        this.loadChronicles().then(() => this.loading = false)
    }

    get loggedInUserIsAuthor() {
        return this.vm.loggedInUser.inRoles.some(role => role == 'Author' || role == 'Admin')
    }
}
</script>

<style lang="scss" scoped>
a {
    cursor: pointer;
}
</style>