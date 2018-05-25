<template>
    <div :class="{ 'col': !showRightSideBar || !showLeftSideBar, 'col-lg-6': showRightSideBar && showLeftSideBar }">
        <post-stream :page-size="pageSize" :paging="paging" :state-stored-items="pagedPosts" :infinite-scroll="infiniteScroll" :loader-predicate="isLoadingPosts">
            <template slot="top">
                <li class="media list-group-item p-4" style="display: block">
                    <div class="flow-root">
                        <a class="float-right large-device" style="margin-top: -1.35rem; margin-right: -1.15rem; cursor: pointer;" v-on:click="toggleRightSideBar">
                            <span class="icon" :class="{ 'icon-chevron-right': showRightSideBar, 'icon-chevron-left': !showRightSideBar }"></span>
                        </a>
                        <a class="float-left large-device" style="margin-top: -1.35rem; margin-left: -1.15rem; cursor: pointer;" v-on:click="toggleLeftSideBar">
                            <span class="icon" :class="{ 'icon-chevron-left': showLeftSideBar, 'icon-chevron-right': !showLeftSideBar }"></span>
                        </a>
                    </div>
                    <new-post  />
                </li>
                <li class="list-group-item list-group-item__no-top-radius p-0">
                    <filter-component class="mb-0" style="border: 0" />
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

import {
    GET_IS_LOADING_POSTS,
    GET_PAGED_POSTS,
    GET_FILTER,
    GET_SHOW_LEFT_SIDEBAR,
    GET_SHOW_RIGHT_SIDEBAR,
    FETCH_PAGED_POSTS,
    SET_IS_LOADING_POSTS,
    SET_FILTER,
    SET_SHOW_LEFT_SIDEBAR,
    SET_SHOW_RIGHT_SIDEBAR
} from '../../../modules/forum/types'

import { Post, Filter } from '../../../model/forum'
import { ForumViewModel, Paging } from '../../../model/common'

import NewPost from '../../../components/post/new.vue'

import PostStream from '../../../components/post/stream.vue'
import FilterComponent from './filter.vue'

import { Pagination } from 'vue-pagination-2'

@Component({
    components: {
        Pagination, NewPost, PostStream, FilterComponent
    }
})
export default class Middle extends Vue {
  	@State(state => state.vm) vm: ForumViewModel
    @ModuleGetter(GET_IS_LOADING_POSTS) isLoadingPosts: boolean
    @ModuleGetter(GET_FILTER) filter: Filter
    @ModuleGetter(GET_PAGED_POSTS) pagedPosts: Paging<Post>
    @ModuleGetter(GET_SHOW_LEFT_SIDEBAR) showLeftSideBar: boolean
    @ModuleGetter(GET_SHOW_RIGHT_SIDEBAR) showRightSideBar: boolean
    
    @ModuleAction(FETCH_PAGED_POSTS) loadPaged: (payload: { pageNumber: number, pageSize: number, filter: Filter, concat: boolean }) => Promise<Paging<Post>>
    @ModuleMutation(SET_IS_LOADING_POSTS) setIsLoadingPosts: (payload: { value: boolean }) => void
    @ModuleMutation(SET_FILTER) setFilter: (payload: { filter: Filter }) => void
    @ModuleMutation(SET_SHOW_LEFT_SIDEBAR) setShowLeftSideBar: (payload: { value: boolean }) => void
    @ModuleMutation(SET_SHOW_RIGHT_SIDEBAR) setShowRightSideBar: (payload: { value: boolean }) => void

    page: number
    private pageSize: number

    infiniteScroll: boolean = true

    created() {
        this.pageSize = this.vm.loggedInUser.filter ? this.vm.loggedInUser.filter.postsPerPage : 20
        this.setFilter({ filter: { excludedUsers: this.vm.loggedInUser.filter.excludedUsers, threadIds: [] }})
    }

    paging(page: number): Promise<Paging<Post>> {
        this.page = page
        this.setIsLoadingPosts({ value: true })
        return new Promise<Paging<Post>>((resolve, reject) => {
            this.loadPaged({ pageNumber: this.page, pageSize: this.pageSize, filter: this.filter, concat: this.infiniteScroll })
                .then((paging: Paging<Post>) => {
                    resolve(paging)
                    this.setIsLoadingPosts({ value: false })
                })
        })
    }

    toggleRightSideBar() {
        this.setShowRightSideBar({ value: !this.showRightSideBar })
    }
    toggleLeftSideBar() {
        this.setShowLeftSideBar({ value: !this.showLeftSideBar })
    }
}
</script>