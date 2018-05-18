<template>
	<ul class="list-group media-list media-list-stream">
		<slot name="top"></slot>
		<post-component v-for="post in paged.data" :key="post.id" :post="post" :full-size="fullSizePost" v-show="infiniteScroll || (!infiniteScroll && !loaderPredicate)" />
		<li class="media list-group-item p-4" v-show="loaderPredicate">
			<loader v-bind="{ background: '#699ED0' }" />
		</li>
		<template v-if="infiniteScroll">
			<li class="media list-group-item p-4" v-if="paged.data.length < paged.total">
				<button class="btn btn-sm btn-success btn-block" @click="load">Ladda fler inlägg</button>
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
import { Component } from 'vue-property-decorator'
import { State, namespace } from 'vuex-class'

import { PageViewModel, Paging } from '../../model/common'
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

	mounted() {
		this.setPage(this.page)
	}

	get paged() {
		return this.stateStoredItems ? this.stateStoredItems : this.staticItems
	}

	setPage(page: number) {
		this.page = page
		if (this.stateStoredItems) {
			this.paging(this.page)
		} else {
			this.paging(this.page).then((pagedItems: Paging<Post>) => {
				this.staticItems = {
					data: this.staticItems.data.length > 0 && this.infiniteScroll ? this.staticItems.data.concat(pagedItems.data) : pagedItems.data,
					currentPage: pagedItems.currentPage,
					numberOfPages: pagedItems.numberOfPages,
					total: pagedItems.total
				}
			})
		}
	}

	load() {
		this.page = this.page + 1
		this.setPage(this.page)
    }
}
</script>