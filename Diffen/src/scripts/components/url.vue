<template>
	<small>
		<a target="_blank" :href="link" v-on:click="click" v-tooltip="link">{{ displayed }}</a>
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

	link: string

	created() {
		if (!this.tip) {
			this.apiSettings = {
				subject: 'post',
				id: this.postId
			}
			this.link = this.href
		} else {
			this.apiSettings = {
				subject: 'tip',
				id: this.tip.id
			}
			this.link = this.tip.href
		}
	}

	get displayed() {
		if (this.text)
			return this.text
		
		let url: string = this.link
		if (this.link.startsWith('http://')) { 
			url = this.link.replace('http://', '')
		}
		else if (this.link.startsWith('https://')) { 
			url = this.link.replace('https://', '')
		}
		if (this.link.includes('unv.is')) {
			url = this.link.replace('https://unv.is/', '')
		}
		return url
	}

	click() {
		this.update({ subject: this.apiSettings.subject, id: this.apiSettings.id })
			.then(() => this.loadUrlTipTopList())
	}
}
</script>