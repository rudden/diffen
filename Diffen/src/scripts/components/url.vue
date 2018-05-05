<template>
	<small>
		<a target="_blank" :href="href" v-on:click="click">{{ displayed }}</a>
	</small>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Action, namespace } from 'vuex-class'

const ModuleAction = namespace('forum', Action)

import { UrlTip } from '../model/forum'

import { UPDATE_URLTIP_CLICKS, FETCH_URLTIP_TOPLIST } from '../modules/forum/types'

@Component({ 
    props: { 
        href: String, 
        text: { 
            type: String,
            default: ''
		},
		postId: Number
    } 
})
export default class Url extends Vue {
	@ModuleAction(UPDATE_URLTIP_CLICKS) update: (payload: { postId: number }) => Promise<boolean>
	@ModuleAction(FETCH_URLTIP_TOPLIST) loadUrlTipTopList: () => Promise<void>

	href: string
	text: string
	postId: number

	get displayed() {
		if (this.text)
			return this.text
			
		let url: string = this.href
		if (this.href.startsWith('http://')) { 
			url = this.href.replace('http://', '')
		}
		else if (this.href.startsWith('https://')) { 
			url = this.href.replace('https://', '')
		}
		if (this.href.includes('unv.is')) {
			url = this.href.replace('https://unv.is/', '')
		}
		return url
	}

	click() {
		this.update({ postId: this.postId })
			.then(() => this.loadUrlTipTopList())
	}
}
</script>