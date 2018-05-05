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

interface Settings {
	subject: string
	id: number
}

@Component({ 
    props: { 
        tip: {
			type: Object,
			default: null	
		},
        text: { 
            type: String,
            default: ''
		},
		href: {
			type: String,
			default: ''
		},
		postId: {
			type: Number,
			default: 0
		}
    } 
})
export default class Url extends Vue {
	@ModuleAction(UPDATE_URLTIP_CLICKS) update: (payload: { subject: string, id: number }) => Promise<boolean>
	@ModuleAction(FETCH_URLTIP_TOPLIST) loadUrlTipTopList: () => Promise<void>

	tip: UrlTip
	text: string
	href: string
	postId: number

	apiSettings: Settings

	created() {
		if (!this.tip) {
			this.apiSettings = {
				subject: 'post',
				id: this.postId
			}
		} else {
			this.apiSettings = {
				subject: 'tip',
				id: this.tip.id
			}
			this.href = this.tip.href
		}
	}

	get displayed() {
		if (this.text)
			return this.text
		
		let href: string = this.tip ? this.tip.href : this.href
		let url: string = href
		if (href.startsWith('http://')) { 
			url = href.replace('http://', '')
		}
		else if (href.startsWith('https://')) { 
			url = href.replace('https://', '')
		}
		if (href.includes('unv.is')) {
			url = href.replace('https://unv.is/', '')
		}
		return url
	}

	click() {
		this.update({ subject: this.apiSettings.subject, id: this.apiSettings.id })
			.then(() => this.loadUrlTipTopList())
	}
}
</script>