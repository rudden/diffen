<template>
    <div>
        <div class="card mb-4" v-if="isSmall">
            <div class="card-body">
                <a href="/kronika/ny" v-if="loggedInUserIsAuthor" style="color: black">
                    <span class="icon icon-plus float-right" v-tooltip="'Skapa ny krönika'"></span>
                </a>
                <h6 class="mb-0">Krönikor</h6>
                <hr />
                <template v-if="!loading">
                    <ul class="list-unstyled list-spaced mb-0">
                        <li class="ellipsis" v-for="chronicle in filtered" :key="chronicle.id">
                            <small>
                                <a :href="`/kronika/${chronicle.slug}`">{{ chronicle.title }}</a>
                            </small>
                        </li>
                    </ul>
                </template>
                <template v-else>
                    <loader v-bind="{ background: '#699ED0' }" />
                </template>
            </div>
        </div>
        <ul class="list-group media-list media-list-stream" v-else>
            <li class="list-group-item p-4"v-if="loggedInUserIsAuthor">
                <a href="/kronika/ny" class="btn btn-sm btn-success float-right" v-if="loggedInUserIsAuthor">Skapa ny krönika</a>
                <h4 class="mb-0">Krönikor</h4>
            </li>
            <li class="media list-group-item p-4">
                <div class="col pl-0 pr-0">
                    <div class="form-group mb-0" :class="{ 'float-right': loggedInUserIsAuthor }">
                        <input type="text" class="form-control form-control-sm" v-model="chronicleSearch" placeholder="Sök">
                    </div>
                    <template v-if="loggedInUserIsAuthor">
                        <div class="form-check form-check-inline">
                            <input class="form-check-input" type="radio" v-model="chroniclesFilter" id="published" value="Published">
                            <label class="form-check-label" for="published">Publicerade</label>
                        </div>
                        <div class="form-check form-check-inline" v-if="loggedInUserIsAdmin">
                            <input class="form-check-input" type="radio" v-model="chroniclesFilter" id="unpublished" value="UnPublished">
                            <label class="form-check-label" for="unpublished">Ej publicerade</label>
                        </div>
                        <div class="form-check form-check-inline">
                            <input class="form-check-input" type="radio" v-model="chroniclesFilter" id="my" value="My">
                            <label class="form-check-label" for="my">Mina</label>
                        </div>
                        <div class="form-check form-check-inline" v-if="loggedInUserIsAdmin">
                            <input class="form-check-input" type="radio" v-model="chroniclesFilter" id="all" value="All">
                            <label class="form-check-label" for="all">Alla</label>
                        </div>
                    </template>
                </div>
            </li>
            <template v-if="!loading">
                <template v-if="filtered.length > 0">
                    <li class="list-group-item media p-4" v-for="chronicle in filtered" :key="chronicle.id">
                        <span class="icon icon-text-document text-muted mr-2"></span>
                        <div class="media-body">
                            <div class="media-heading">
                                <a :href="`/kronika/${chronicle.slug}`">
                                    <strong>{{ chronicle.title }}</strong>
                                    <span class="badge badge-danger ml-2" v-if="chronicle.published > today">inte publicerad än</span>
                                </a>
                            </div>
                            <div>
                                <small class="text-muted float-right">{{ chronicle.published }}</small>
                                <small class="text-muted">Av: </small><small><a :href="`/profil/${chronicle.writtenByUser.id}`">{{ chronicle.writtenByUser.nickName }}</a></small>
                            </div>
                        </div>
                    </li>
                </template>
                <template v-else>
                    <li class="list-group-item media p-4">
                        <div class="media-body">
                            <div class="alert alert-warning mb-0">Hittade inga krönikor</div>
                        </div>
                    </li>
                </template>
            </template>
            <template v-else>
                <li class="media list-group-item p-4">
                    <loader v-bind="{ background: '#699ED0' }" />
                </li>
            </template>
        </ul>
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component, Watch } from 'vue-property-decorator'
import { Getter, Action, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('other', Getter)
const ModuleAction = namespace('other', Action)

import { PageViewModel } from '../../model/common'
import { Chronicle } from '../../model/other'

import { GET_CHRONICLES, FETCH_CHRONICLES } from '../../modules/other/types'

import Modal from '../modal.vue'

enum ChroniclesFilter {
    All, My, Published, UnPublished
}

@Component({
    props: {
        isSmall: {
            type: Boolean,
            default: false
        },
        amountOfChronicles: {
            type: Number,
            default: 0
        }
    },
	components: {
        Modal
	}
})
export default class Chronicles extends Vue {
    @State(state => state.vm) vm: PageViewModel
    @ModuleGetter(GET_CHRONICLES) chronicles: Chronicle[]
    @ModuleAction(FETCH_CHRONICLES) loadChronicles: (payload: { amount: number }) => Promise<void>

    isSmall: boolean
    amountOfChronicles: number

    loading: boolean = true

    chroniclesFilter: string = ''
    filteredChronicles: Chronicle[] = []
    chronicleSearch: string = ''

    today: string = new Date().toISOString().slice(0, 10)

	mounted() {
        this.loadChronicles({ amount: this.amountOfChronicles }).then(() => {
            this.chroniclesFilter = ChroniclesFilter[ChroniclesFilter.Published]
            this.loading = false
        })
    }

    @Watch('chroniclesFilter')
        onChange() {
            let selected = ChroniclesFilter[this.chroniclesFilter as keyof typeof ChroniclesFilter]
            switch (selected) {
                case ChroniclesFilter.My:
                    this.filteredChronicles = this.chronicles.filter((c: Chronicle) => c.writtenByUser.id == this.vm.loggedInUser.id)
                    break
                case ChroniclesFilter.Published:
                    this.filteredChronicles = this.chronicles.filter((c: Chronicle) => c.published <= this.today)
                    break
                case ChroniclesFilter.UnPublished:
                    this.filteredChronicles = this.chronicles.filter((c: Chronicle) => c.published > this.today)
                    break
                case ChroniclesFilter.All:
                    this.filteredChronicles = this.chronicles
                    break
            }
        }
    
    get filtered() {
		return this.filteredChronicles.filter((c: Chronicle) => {
			return c.title.toLowerCase().includes(this.chronicleSearch.toLowerCase())
		})
	}

    get loggedInUserIsAuthor() {
        return this.vm.loggedInUser.inRoles.some(role => role == 'Author' || role == 'Admin')
    }

     get loggedInUserIsAdmin() {
        return this.vm.loggedInUser.inRoles.some(role => role == 'Admin')
    }
}
</script>

<style lang="scss" scoped>
a {
    cursor: pointer;
}
</style>