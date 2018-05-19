<template>
	<div class="container container__profile mt-3 mb-5">
		<div class="row">
			<div class="col-sm-12 col-md col-lg">
				<post-stream :full-size-post="false" :paging="pageCreatedPosts" :state-stored-items="createdPosts" :loader-predicate="isLoadingCreatedPosts">
					<template slot="top">
						<li class="media list-group-item p-4">
							<h6 class="mb-0">Skapade inlägg</h6>
						</li>
					</template>
				</post-stream>
			</div>
			<div class="col-sm-12 col-md col-lg" v-if="vm.loggedInUser.id == userId">
				<post-stream :full-size-post="false" :paging="pageSavedPosts" :state-stored-items="savedPosts" :loader-predicate="isLoadingSavedPosts">
					<template slot="top">
						<li class="media list-group-item p-4">
							<h6 class="mb-0">Sparade inlägg</h6>
						</li>
					</template>
				</post-stream>
			</div>
		</div>
	</div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Action, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('profile', Getter)
const ModuleAction = namespace('profile', Action)

import { Post } from '../../../model/forum'
import { ProfileViewModel, Paging } from '../../../model/common'

import { GET_CREATED_POSTS, GET_SAVED_POSTS, FETCH_POSTS, FETCH_SAVED_POSTS } from '../../../modules/profile/types'

import PostStream from '../../../components/post/stream.vue'

import { Pagination } from 'vue-pagination-2'

@Component({
	components: {
		Pagination, PostStream
	}
})
export default class Posts extends Vue {
	@State(state => state.vm) vm: ProfileViewModel
	@ModuleGetter(GET_CREATED_POSTS) createdPosts: Paging<Post>
	@ModuleGetter(GET_SAVED_POSTS) savedPosts: Paging<Post>
	@ModuleAction(FETCH_POSTS) loadPosts: (payload: { userId: string, pageNumber: number }) => Promise<void>
	@ModuleAction(FETCH_SAVED_POSTS) loadSavedPosts: (payload: { userId: string, pageNumber: number }) => Promise<void>

	pageForPostsCreatedBySelectedUser: number = 1
	pageForPostsSavedBySelectedUser: number = 1

	isLoadingCreatedPosts: boolean = false
	isLoadingSavedPosts: boolean = false

	private userId: string

	created() {
		this.userId = this.vm.selectedUserId ? this.vm.selectedUserId : this.vm.loggedInUser.id
	}

    pageCreatedPosts(page: number) {
		this.isLoadingCreatedPosts = true
        this.pageForPostsCreatedBySelectedUser = page
		return this.loadPosts({ userId: this.userId, pageNumber: this.pageForPostsCreatedBySelectedUser })
			.then(() => this.isLoadingCreatedPosts = false)
	}
	
	pageSavedPosts(page: number) {
		this.isLoadingSavedPosts = true
		this.pageForPostsSavedBySelectedUser = page
		return this.loadSavedPosts({ userId: this.userId, pageNumber: this.pageForPostsSavedBySelectedUser })
			.then(() => this.isLoadingSavedPosts = false)
    }
}
</script>
