<template>
    <div>
        <div class="input-group">
            <textarea rows="5" class="form-control" placeholder="Ditt inlägg.." v-model="newPost.message"></textarea>
        </div>
        <div class="col mt-3 mb-3 pl-0 pr-0">
            <template v-if="lineups.length > 0 || noLineupsFound">
                <select class="form-control" v-model="newPost.lineupId" :disabled="!lineups.length > 0" @change="changeLineup">
                    <option value="0">{{ lineups.length > 0 ? 'Välj en startelva' : 'Hittade inga startelvor' }}</option>
                    <option v-for="lineup in lineups" :value="lineup.id" :key="lineup.id">{{ lineup.formation.name }}, skapad {{ lineup.created }}</option>
                </select>
            </template>
            <template v-else>
                <button class="btn btn-primary btn-sm btn-block" v-on:click="fetchLineups">Ladda startelvor</button>
            </template>
        </div>
        <div class="input-group">
            <input type="text" class="form-control" placeholder="Länktips" v-model="newPost.urlTipHref">
            <div class="input-group-btn">
                <button class="btn btn-success align-self-stretch ml-2" v-on:click="submit" :disabled="!canSubmit">{{ btnText }}</button>
            </div>
        </div>
        <template v-if="newPost.lineupId > 0">
            <div class="mt-3">
                <formation-component :formation="selectedLineup.formation" :players="selectedLineup.players" />
            </div>
        </template>
        <results :items="results" class="pt-3" />
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component, Watch } from 'vue-property-decorator'
import { Getter, Mutation, Action, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('forum', Getter)
const ModuleAction = namespace('forum', Action)
const ModuleMutation = namespace('forum', Mutation)

const SquadModuleGetter = namespace('squad', Getter)
const SquadModuleAction = namespace('squad', Action)
const SquadModuleMutation = namespace('squad', Mutation)

import { GET_FILTER, CREATE_POST, UPDATE_POST, FETCH_PAGED_POSTS, SET_IS_LOADING_POSTS, SET_SHOULD_RELOAD_POST_STREAM } from '../../modules/forum/types'
import { GET_LINEUPS, GET_SELECTED_LINEUP, FETCH_LINEUPS_ON_USER, SET_SELECTED_LINEUP } from '../../modules/squad/types'

import { Post, Filter } from '../../model/forum'
import { Post as CrudPost } from '../../model/forum/crud'
import { PageViewModel, Result, ResultType, Paging } from '../../model/common'
import { Lineup, PlayerToLineup } from '../../model/squad'
import { Lineup as CrudLineup } from '../../model/squad/crud'

import Results from '../results.vue'
import FormationComponent from '../lineups/formation.vue'

@Component({
    props: {
        post: Object,
        parentId: {
            type: Number,
            default: 0
        },
        modalName: {
            type: String,
            default: undefined
        }
    },
    components: {
        Results, FormationComponent
    }
})
export default class NewPost extends Vue {
  	@State(state => state.vm) vm: PageViewModel
    @ModuleGetter(GET_FILTER) filter: Filter
    @ModuleAction(CREATE_POST) create: (payload: { post: CrudPost }) => Promise<Result[]>
    @ModuleAction(UPDATE_POST) update: (payload: { post: CrudPost }) => Promise<Result[]>
	@ModuleAction(FETCH_PAGED_POSTS) loadPaged: (payload: { pageNumber: number, pageSize: number, filter: Filter }) => Promise<void>
	@ModuleMutation(SET_IS_LOADING_POSTS) setIsLoadingPosts: (payload: { value: boolean }) => void
	@ModuleMutation(SET_SHOULD_RELOAD_POST_STREAM) setShouldReloadPostStream: (payload: { value: boolean }) => void

	@SquadModuleGetter(GET_LINEUPS) lineups: Lineup[]
    @SquadModuleGetter(GET_SELECTED_LINEUP) selectedLineup: Lineup
	@SquadModuleAction(FETCH_LINEUPS_ON_USER) loadLineups: (payload: { userId: string }) => Promise<void>
	@SquadModuleMutation(SET_SELECTED_LINEUP) setSelectedLineup: (lineup: Lineup) => void
    
    post: Post
    parentId: number
    modalName: string

    loading: boolean = true
    noLineupsFound: boolean = false
    newPost: CrudPost = new CrudPost()
    results: Result[] = []

    $modal = (this as any).VModal

    created() {
        if (this.post) {
            this.newPost.id = this.post.id
            this.newPost.message = this.post.message
            this.newPost.urlTipHref = this.post.urlTipHref
            if (this.post.lineupId) {
                this.fetchLineups()
                    .then(() => {
                        this.newPost.lineupId = this.post.lineupId ? this.post.lineupId : 0
                        this.changeLineup()
                    })
            } else {
                this.newPost.lineupId = 0
            }
        }
        this.newPost.parentPostId = this.parentId == 0 ? undefined : this.parentId
        this.newPost.createdByUserId = this.vm.loggedInUser.id
    }

    get btnText(): string {
        if (!this.post)
            return 'Skicka'
        return 'Spara'
    }
    get canSubmit() {
        return this.newPost.message ? this.newPost.message.length > 0 ? true : false : false
    }
    get inEditMode() {
        return this.post && this.post.inEdit ? true : false
    }
    get inReplyMode() {
        return this.parentId > 0 ? true : false
    }

    @Watch('results')
        onChange() {
            this.setShouldReloadPostStream({ value: true })
        }

    fetchLineups() {
		return this.loadLineups({ userId: this.vm.loggedInUser.id }).then(() => this.noLineupsFound = this.lineups.length == 0)
    }
    
    changeLineup() {
		if (this.newPost.lineupId > 0) 
			this.setSelectedLineup(this.lineups.filter((l: Lineup) => l.id == this.newPost.lineupId)[0])
		else
			this.setSelectedLineup(new Lineup())
	}

    submit() {
        if (this.inEditMode || this.inReplyMode && this.modalName) {
            this.$modal.hide(this.modalName)
        }
        this.setIsLoadingPosts({ value: true })
        new Promise<Result[]>((resolve, reject) => {
            if (this.inEditMode) {
                this.update({ post: this.newPost }).then((res) => resolve(res)).catch(() => resolve([{ type: ResultType.Failure, message: 'Kunde inte uppdatera inlägget' }]))
            }
            else {
                this.create({ post: this.newPost }).then((res) => resolve(res)).catch(() => resolve([{ type: ResultType.Failure, message: 'Kunde inte svara på inlägget' }]))
            }
        }).then((res) => {
            this.results = res
            this.newPost = new CrudPost()
            this.loadPaged({ pageNumber: 1, pageSize: this.vm.loggedInUser.filter.postsPerPage, filter: this.filter })
                .then(() => this.setIsLoadingPosts({ value: false }))
        })
    }
}
</script>

<style lang="scss" scoped>
.input-group:nth-child(2) {
    padding-top: 1.5rem;
    input, select, option {
        border-radius: 4px !important;
    }
}
</style>
