<template>
    <div class="col-lg-6">
        <ul class="list-group media-list media-list-stream mb-4">
            <li class="media list-group-item p-4" style="display: block">
                <new-post />
            </li>
            <li class="media list-group-item p-4" v-show="isLoadingPosts">
                laddar inlägg..
            </li>
            <div v-show="!isLoadingPosts">
                <post-component v-for="post in pagedPosts.data" :key="post.id" :post="post" />
                <template v-if="pagedPosts.total && pagedPosts.total > 1">
                    <div class="mt-3">
                        <pagination @paginate="setPage" 
                            v-bind="{
                                records: pagedPosts.total,
                                perPage: pageSize,
                                options: {
                                    theme: 'bootstrap4',
                                    texts: {
                                        count: 'visar {from} till {to} av {count} inlägg|{count} inlägg|ett inlägg'
                                    }
                                }
                            }" />
                    </div>
                </template>
            </div>
        </ul>
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Mutation, Action, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('forum', Getter)
const ModuleAction = namespace('forum', Action)
const ModuleMutation = namespace('forum', Mutation)

import { GET_PAGED_POSTS, GET_FILTER, GET_IS_LOADING_POSTS, FETCH_PAGED_POSTS, SET_IS_LOADING_POSTS } from '../../../modules/forum/types'

import { Post, Filter } from '../../../model/forum'
import { ViewModel, Paging } from '../../../model/common'

import NewPost from './new-post.vue'
import PostComponent from './post.vue'
import Url from '../../../components/url.vue'

import { Pagination } from 'vue-pagination-2'

@Component({
    components: {
        NewPost, PostComponent, Url, Pagination
    }
})
export default class Posts extends Vue {
  	@State(state => state.vm) vm: ViewModel
	@ModuleGetter(GET_PAGED_POSTS) pagedPosts: Paging<Post>
	@ModuleGetter(GET_IS_LOADING_POSTS) isLoadingPosts: boolean
	@ModuleGetter(GET_FILTER) filter: Filter
	@ModuleAction(FETCH_PAGED_POSTS) loadPaged: (payload: { pageNumber: number, pageSize: number, filter: Filter }) => Promise<void>
	@ModuleMutation(SET_IS_LOADING_POSTS) setIsLoadingPosts: (payload: { value: boolean }) => void

	page: number = 1
    private pageSize: number

	mounted() {
        this.pageSize = this.vm.loggedInUser.filter ? this.vm.loggedInUser.filter.postsPerPage : 1
        this.paging()
    }
    
    setPage(page: number) {
		this.page = page
		this.setIsLoadingPosts({ value: true })
		this.paging()
    }
    
    paging() {
        this.loadPaged({ pageNumber: this.page, pageSize: this.pageSize, filter: this.filter })
			.then(() => this.setIsLoadingPosts({ value: false }))
    }
}
</script>

<style lang="scss" scoped>
</style>
