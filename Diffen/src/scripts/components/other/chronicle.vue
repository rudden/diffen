<template>
    <div class="row">
        <div class="col-md-9">
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
                        <li class="list-group-item pt-2 pb-2">
                            <span class="badge badge-dark mr-1" v-for="category in chronicle.categories" :key="category.id">
                                <span class="icon icon-tag mr-1"></span>
                                {{ category.name }}
                            </span>
                        </li>
                        <li class="list-group-item media p-0">
                            <img :src="'/' + chronicle.headerFileName" class="img-fluid mx-auto" data-action="zoom">
                        </li>
                        <li class="list-group-item media">
                            <div class="media-body">
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
        </div>
        <div class="col-md-3">
            <div class="card card-profile mb-4" v-if="!loading">
                <div class="card-header" style="background-image: url(/bg.jpg);"></div>
                <div class="card-body text-center">
                    <a href="/profil">
                        <img class="card-profile-img" :src="chronicle.writtenByUser.avatar">
                    </a>
                    <h6 class="card-title">
                        <a class="text-inherit" href="/profile">{{ chronicle.writtenByUser.nickName }}</a>
                    </h6>
                    <p class="mb-4" v-if="chronicle.writtenByUser.bio">{{ chronicle.writtenByUser.bio }}</p>
                    <ul class="card-menu">
                        <li class="card-menu-item">
                            Medlem sedan
                            <h6 class="my-0">{{ chronicle.writtenByUser.joined }}</h6>
                        </li>
                        <li class="card-menu-item">
                            Antal krönikor
                            <h6 class="my-0">{{ createdByUsersAmountOfChronicles }}</h6>
                        </li>
                    </ul>
                </div>
            </div>
            <div class="card mb-4">
                <div class="card-body">
                    <h6>Senaste</h6>
                    <hr />
                    <ul class="list-unstyled list-spaced mb-0">
                        <li class="ellipsis" v-for="chronicle in lastChronicles" :key="chronicle.id">
                            <a :href="`/kronika/${chronicle.slug}`">{{ chronicle.title }}</a>
                        </li>
                    </ul>
                </div>
            </div>
            <div class="card mb-4">
                <div class="card-body">
                    <h6>Relaterade</h6>
                    <hr />
                    <ul class="list-unstyled list-spaced mb-0">
                        <li class="ellipsis" v-for="chronicle in relatedChronicles" :key="chronicle.id">
                            <a :href="`/kronika/${chronicle.slug}`">{{ chronicle.title }}</a>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Action, Mutation, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('other', Getter)
const ModuleAction = namespace('other', Action)
const ModuleMutation = namespace('other', Mutation)

import { ChronicleViewModel } from '../../model/common'
import { Chronicle, ChronicleCategory } from '../../model/other'

import { GET_CHRONICLE, GET_CHRONICLES, FETCH_CHRONICLE, FETCH_CHRONICLES } from '../../modules/other/types'

import SmallUser from '../small-user.vue'

@Component({
    components: {
        SmallUser
    }
})
export default class ChronicleComponent extends Vue {
    @State(state => state.vm) vm: ChronicleViewModel
    @ModuleGetter(GET_CHRONICLE) chronicle: Chronicle
    @ModuleGetter(GET_CHRONICLES) chronicles: Chronicle[]
    @ModuleAction(FETCH_CHRONICLE) loadChronicle: (payload: { slug: string }) => Promise<void>
    @ModuleAction(FETCH_CHRONICLES) loadChronicles: (payload: { amount: number }) => Promise<void>

    loading: boolean = true

	mounted() {
        this.loadChronicles({ amount: 0 })
        this.loadChronicle({ slug: this.vm.selectedChronicleSlug })
            .then(() => this.loading = false)
    }

    get createdByLoggedInUser() {
        return this.vm.loggedInUser.id == this.chronicle.writtenByUser.id
    }

    get lastChronicles() {
        return this.chronicles.slice(0, 10)
    }

    // based on chronicles with same category
    get relatedChronicles() {
        return this.chronicles.filter((c: Chronicle) => 
            c.categories.map((c: ChronicleCategory) => c.id).some(id => 
                this.chronicle.categories.map((c: ChronicleCategory) => 
                    c.id).includes(id))).slice(0, 10)
    }

    get createdByUsersAmountOfChronicles() {
        return this.chronicles.filter((c: Chronicle) => c.writtenByUser.id == this.chronicle.writtenByUser.id).length
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