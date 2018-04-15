<template>
    <div>
        <div class="input-group">
            <textarea rows="5" class="form-control" placeholder="ditt inlägg.." v-model="newPost.message"></textarea>
        </div>
        <div class="input-group">
            <input type="text" class="form-control" placeholder="länktips" v-model="newPost.urlTipHref">
            <div class="input-group-btn">
                <button class="btn btn-success align-self-stretch ml-2" v-on:click="submit" :disabled="!canSubmit">{{ btnText }}</button>
            </div>
        </div>
        <div class="mt-3">
            <lineups v-bind="{ preSelectedLineupId: post ? post.lineupId ? post.lineupId : 0 : 0 }" />
        </div>
        <results :items="results" :dismiss="dismiss" class="pt-3" />
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Mutation, Action, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('forum', Getter)
const ModuleAction = namespace('forum', Action)
const ModuleMutation = namespace('forum', Mutation)

const SquadModuleGetter = namespace('squad', Getter)

import { GET_FILTER, CREATE_POST, UPDATE_POST, FETCH_PAGED_POSTS, SET_IS_LOADING_POSTS } from '../../modules/forum/types'
import { GET_SELECTED_LINEUP } from '../../modules/squad/types'

import { Post, Filter } from '../../model/forum'
import { Post as CrudPost } from '../../model/forum/crud'
import { ViewModel, Result, ResultType, Paging } from '../../model/common'
import { Lineup, PlayerToLineup } from '../../model/squad'
import { Lineup as CrudLineup } from '../../model/squad/crud'

import Results from '../results.vue'
import Lineups from '../lineups/lineups.vue'

@Component({
    props: {
        post: Object,
        parentId: {
            type: Number,
            default: 0
        }
    },
    components: {
        Results, Lineups
    }
})
export default class NewPost extends Vue {
  	@State(state => state.vm) vm: ViewModel
    @ModuleGetter(GET_FILTER) filter: Filter
    @ModuleAction(CREATE_POST) create: (payload: { post: CrudPost }) => Promise<Result[]>
    @ModuleAction(UPDATE_POST) update: (payload: { post: CrudPost }) => Promise<Result[]>
	@ModuleAction(FETCH_PAGED_POSTS) loadPaged: (payload: { pageNumber: number, pageSize: number, filter: Filter }) => Promise<void>
	@ModuleMutation(SET_IS_LOADING_POSTS) setIsLoadingPosts: (payload: { value: boolean }) => void

    @SquadModuleGetter(GET_SELECTED_LINEUP) selectedLineup: Lineup

    post: Post
    parentId: number

    loading: boolean = true
    newPost: CrudPost = new CrudPost()
    results: Result[] = []

    created() {
        if (this.post) {
            this.newPost.id = this.post.id
            this.newPost.message = this.post.message
            this.newPost.urlTipHref = this.post.urlTipHref
        }
        this.newPost.parentPostId = this.parentId == 0 ? undefined : this.parentId
        this.newPost.createdByUserId = this.vm.loggedInUser.id
    }

    get btnText(): string {
        if (!this.post)
            return 'skapa inlägg'
        return 'spara inlägg'
    }
    get canSubmit() {
        return this.newPost.message ? this.newPost.message.length > 0 ? true : false : false
    }
    get lineup(): CrudLineup {
        return {
            id: this.selectedLineup.id,
            players: this.selectedLineup.players.map((ptl: PlayerToLineup) => {
                return {
                    playerId: ptl.player.id,
                    positionId: ptl.position.id
                }
            }),
            createdByUserId: this.vm.loggedInUser.id,
            formationId: this.selectedLineup.formation.id,
            created: this.selectedLineup.created
        }
    }

    submit() {
        this.setIsLoadingPosts({ value: true })
        new Promise<Result[]>((resolve, reject) => {
            if (this.newPost.id > 0) {
                if (!this.selectedLineup.id) {
                    this.newPost.lineup = undefined
                } else {
                    if (this.post.lineupId !== this.selectedLineup.id) {
                        this.newPost.lineup = this.lineup
                    }
                }
                if (this.post.urlTipHref == this.newPost.urlTipHref) {
                    this.newPost.urlTipHref = undefined
                }
                this.update({ post: this.newPost }).then((res) => resolve(res))
            }
            else {
                if (this.selectedLineup.id) {
                    this.newPost.lineup = this.lineup
                }
                this.create({ post: this.newPost })
                    .then((res) => {
                        resolve(res)
                        this.newPost = new CrudPost()
                    })
            }
        }).then((res) => {
            this.results = res
            this.loadPaged({ pageNumber: 1, pageSize: this.vm.loggedInUser.filter.postsPerPage, filter: this.filter })
                .then(() => this.setIsLoadingPosts({ value: false }))
        })
    }

    dismiss(type: ResultType) {
		this.results = this.results.filter((r: Result) => r.type != type)
	}
}
</script>

<style lang="scss" scoped>
.input-group:nth-child(2) {
    padding-top: 1.5rem;
    input {
        border-radius: 4px !important;
    }
}
</style>
