<template>
    <div class="embeds" v-if="hasEmbed">
        <template v-if="videoSrc">
            <div class="embed-responsive embed-responsive-16by9 mt-3">
                <iframe class="embed-responsive-item" v-bind:src="videoSrc" allowfullscreen></iframe>
            </div>
        </template>
        <template v-if="twitterSrc">
            <tweet :id="twitterSrc" />
        </template>
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'

import { Tweet } from 'vue-tweet-embed'

@Component({
    props: {
        href: String
    },
    components: {
        Tweet
    }
})
export default class Embeds extends Vue {
    href: string

    get videoSrc(): string { 
        return this.href ? this.href.includes('youtube.com') ? '//www.youtube.com/embed/' + this.href.split('?v=').pop() + '?rel=0' : '' : ''
    }
	get twitterSrc() { 
        return this.href ? this.href.includes('twitter.com') ? this.href.split('status/').pop() : '' : '' 
    }
    get hasEmbed(): boolean {
        return this.videoSrc || this.twitterSrc ? true : false
    }
}
</script>