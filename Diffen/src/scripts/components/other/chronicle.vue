<template>
    <ul class="list-group media-list media-list-stream">
        <li class="media list-group-item p-4" v-show="loading">
            <loader v-bind="{ background: '#699ED0' }" />
        </li>
        <div v-show="!loading">
            <template v-if="!loading">
                <li class="list-group-item p-4">
                    <a :href="`/kronika/uppdatera/${chronicle.slug}`" v-if="createdByLoggedInUser" class="float-right">
                        <span class="icon icon-pencil"></span>
                    </a>
                    <h4 class="mb-0">{{ chronicle.title }}</h4>
                </li>
                <li class="list-group-item media p4">
                    <div class="media-body">
                        <img :src="'/' + chronicle.headerFileName" class="img-fluid mt-2" data-action="zoom">
                        <div v-html="chronicle.text"></div>
                        <hr class="divider" />
                        <div class="pt-2 pb-2" style="display: flow-root">
                            <small class="text-muted float-right">{{ chronicle.created }}</small>
                            <span class="float-left">Av: <a :href="`/profil/${chronicle.writtenByUser.id}`">{{ chronicle.writtenByUser.nickName }}</a></span>
                        </div>
                    </div>
                </li>
            </template>
        </div>
    </ul>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Action, Mutation, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('other', Getter)
const ModuleAction = namespace('other', Action)
const ModuleMutation = namespace('other', Mutation)

import { ChronicleViewModel } from '../../model/common'
import { Chronicle } from '../../model/other'

import { GET_CHRONICLE, FETCH_CHRONICLE } from '../../modules/other/types'

@Component({})
export default class ChronicleComponent extends Vue {
    @State(state => state.vm) vm: ChronicleViewModel
    @ModuleGetter(GET_CHRONICLE) chronicle: Chronicle
    @ModuleAction(FETCH_CHRONICLE) loadChronicle: (payload: { slug: string }) => Promise<void>

    loading: boolean = true

	mounted() {
        this.loadChronicle({ slug: this.vm.selectedChronicleSlug })
            .then(() => this.loading = false)
    }

    get createdByLoggedInUser() {
        return this.vm.loggedInUser.id == this.chronicle.writtenByUser.id
    }
}
</script>

<style lang="scss" scoped>
a {
    cursor: pointer;
}
hr.divider {
    margin-left: -15px;
    margin-right: -15px;
}
</style>