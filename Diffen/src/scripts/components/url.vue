<template>
	<small>
		<a target="_blank" :href="href">{{ displayed }}</a>
	</small>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'

@Component({ 
    props: { 
        href: String, 
        trim: { 
            type: Boolean, 
            default: false 
        }, 
        text: { 
            type: String,
            default: ''
        } 
    } 
})
export default class Url extends Vue {
	href: string
	trim: boolean
	text: string

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
		if (this.trim) {
			if (url.length > 30) {
				url = url.substring(0, 30) + '...'
			}
		}
		return url
	}
}
</script>