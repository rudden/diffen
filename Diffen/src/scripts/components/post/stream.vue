<template>
	<ul class="list-group media-list media-list-stream">
		<slot name="top"></slot>
		<li class="media list-group-item p-4" v-if="selectedPageNumber > 0 && inSinglePageView">
			Visar sida {{ selectedPageNumber }}
		</li>
		<post-component v-for="post in paged.data" :key="post.id" :post="post" :full-size="fullSizePost" v-show="infiniteScroll || (!infiniteScroll && !loaderPredicate)" />
		<li class="media list-group-item p-4" v-show="loaderPredicate">
			<loader v-bind="{ background: '#699ED0' }" />
		</li>
		<template v-if="infiniteScroll">
			<li class="media list-group-item p-4" v-if="paged.data.length < paged.total">
				<div class="input-group">
					<button class="btn btn-sm btn-success mr-3" style="width: 65%" v-on:click="load" v-if="!isLastPage" :disabled="loaderPredicate">Ladda fler inlägg</button>
					<select class="form-control form-control-sm" :disabled="loaderPredicate" v-model="selectedPageNumber">
						<option v-for="page in pages" :value="page">Visa sida {{ page }}</option>
					</select>
				</div>
			</li>
			<li class="media list-group-item p-4" v-else-if="!loaderPredicate && (paged.data.length == 0 || paged.data.length == paged.total)">
				{{ paged.data.length == 0 ? 'Hittade inga inlägg' : paged.data.length == paged.total ? 'Inga fler inlägg' : '' }}
			</li>
		</template>
		<template v-else>
			<li class="media list-group-item p-4" v-show="paged.data.length <= 0">
				Hittade inga inlägg
			</li>
		</template>
		<template v-if="!infiniteScroll && (paged.total && paged.total > 1)">
			<div class="mt-3">
				<pagination @paginate="setPage" 
					v-bind="{
						records: paged.total,
						perPage: pageSize,
						options: {
							chunk: 5,
							theme: 'bootstrap4',
							edgeNavigation: true,
							texts: {
								first: 'första',
								last: 'sista',
								count: 'visar {from} till {to} av {count} inlägg|{count} inlägg|ett inlägg'
							}
						}
					}" />
			</div>
		</template>
	</ul>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component, Watch } from 'vue-property-decorator'
import { State, namespace } from 'vuex-class'

import { PageViewModel, ForumViewModel, Paging } from '../../model/common'
import { Post } from '../../model/forum'

import PostComponent from './item.vue'

import { Pagination } from 'vue-pagination-2'

@Component({
	props: {
		fullSizePost: {
			type: Boolean,
			default: true
		},
		pageSize: {
			type: Number,
			default: 5
		},
		stateStoredItems: Object,
		loaderPredicate: Boolean,
		infiniteScroll: {
			type: Boolean,
			default: false
		},
		paging: Function
	},
	components: {
		PostComponent, Pagination
	}
})
export default class PostStream extends Vue {
	@State(state => state.vm) vm: PageViewModel

	fullSizePost: boolean
	pageSize: number
	stateStoredItems: Paging<Post>
	loaderPredicate: boolean
	infiniteScroll: boolean
	paging: (page: number) => Promise<Paging<Post>>

	page: number = 1
	staticItems: Paging<Post> = new Paging<Post>()

	pages: number[] = []
	selectedPageNumber: number = 0

    private baseUrl: string

	mounted() {
		var getUrl = window.location
		this.baseUrl = getUrl .protocol + '//' + getUrl.host + '/' + getUrl.pathname.split('/')[0]

		this.selectedPageNumber = this.currentPage
		this.page = this.selectedPageNumber == 0 ? 1 : this.selectedPageNumber
		this.setPage(this.page)
	}

	@Watch('selectedPageNumber')
		onChange() {
			if (this.selectedPageNumber > 0) {
				if (this.selectedPageNumber == 1 && !this.inSinglePageView)
					return

				let newUrl: string = `${this.baseUrl}forum/sida/${this.selectedPageNumber}`
				if (window.location.href == newUrl)
					return
				this.redirectTo(newUrl)
			} else {
				this.redirectTo(`${this.baseUrl}forum`)
			}
		}

	get paged() {
		return this.stateStoredItems ? this.stateStoredItems : this.staticItems
	}
	get inForumView(): boolean {
		return this.vm.page == 'forum' ? true : false
	}
	get isLastPage() {
		let lastPage: number = <number>this.pages.slice(-1).pop()
		return this.selectedPageNumber == lastPage || this.page == lastPage
	}
	get inSinglePageView() {
		return window.location.toString().includes('forum/sida/')
	}
	get currentPage() {
		if (this.inForumView) {
			let pageNumber: number = (this.vm as ForumViewModel).selectedPageNumber
			return pageNumber == 0 ? this.page : pageNumber
		}
		return this.page
	}

	setPage(page: number) {
		this.page = page
		return new Promise<void>((resolve, reject) => {
			if (this.stateStoredItems) {
				this.paging(this.page).then(() => resolve())
			} else {
				this.paging(this.page).then((pagedItems: Paging<Post>) => {
					this.staticItems = {
						data: this.staticItems.data.length > 0 && this.infiniteScroll ? this.staticItems.data.concat(pagedItems.data) : pagedItems.data,
						currentPage: pagedItems.currentPage,
						numberOfPages: pagedItems.numberOfPages,
						total: pagedItems.total
					}
					resolve()
				})
			}
		}).then(() => {
			this.pages = []
			for (let i = 1; i < this.paged.numberOfPages + 1; i++) {
				this.pages.push(i)
			}
		})
	}

	load() {
		this.page = this.page + 1
		this.setPage(this.page)
	}
	
	redirectTo(href: string): void {
		window.location.href = href
	}
}
</script>