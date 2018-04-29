<template>
    <div class="col-lg-6">
        <post-stream :page-size="pageSize" :state-stored-items="pagedPosts" :paging="paging" :loader-predicate="isLoadingPosts">
            <template slot="top">
                <li class="media list-group-item p-4" style="display: block">
                    <new-post  />
                </li>
            </template>
        </post-stream>
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Action, Mutation, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('forum', Getter)
const ModuleAction = namespace('forum', Action)
const ModuleMutation = namespace('forum', Mutation)

import { GET_IS_LOADING_POSTS, GET_PAGED_POSTS, GET_FILTER, FETCH_PAGED_POSTS, SET_IS_LOADING_POSTS, SET_FILTER } from '../../../modules/forum/types'

import { Post, Filter } from '../../../model/forum'
import { PageViewModel, Paging } from '../../../model/common'

import NewPost from '../../../components/post/new.vue'

import PostStream from '../../../components/post/stream.vue'

import { Pagination } from 'vue-pagination-2'

@Component({
    components: {
        Pagination, NewPost, PostStream
    }
})
export default class Middle extends Vue {
  	@State(state => state.vm) vm: PageViewModel
    @ModuleGetter(GET_IS_LOADING_POSTS) isLoadingPosts: boolean
    @ModuleGetter(GET_FILTER) filter: Filter
    @ModuleGetter(GET_PAGED_POSTS) pagedPosts: Paging<Post>
    @ModuleAction(FETCH_PAGED_POSTS) loadPaged: (payload: { pageNumber: number, pageSize: number, filter: Filter }) => Promise<Paging<Post>>
    @ModuleMutation(SET_IS_LOADING_POSTS) setIsLoadingPosts: (payload: { value: boolean }) => void
    @ModuleMutation(SET_FILTER) setFilter: (payload: { filter: Filter }) => void

    page: number = 1
    private pageSize: number

    created() {
        this.pageSize = this.vm.loggedInUser.filter ? this.vm.loggedInUser.filter.postsPerPage : 1
        this.setFilter({ filter: { excludedUsers: this.vm.loggedInUser.filter.excludedUsers }})
    }

    paging(page: number) {
        this.page = page
        this.setIsLoadingPosts({ value: true })
        this.loadPaged({ pageNumber: this.page, pageSize: this.pageSize, filter: this.filter })
            .then(() => this.setIsLoadingPosts({ value: false }))
    }
}
</script>