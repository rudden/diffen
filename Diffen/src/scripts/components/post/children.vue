<template>
    <ul class="media-list">
        <li class="media" v-for="child in children">
            <a :href="'/profil/' + child.post.user.id">
                <img class="media-object mr-3 align-self-start" :src="child.post.user.avatar">
            </a>
            <div class="media-body">
                <small class="float-right text-muted">
                    <span class="icon icon-check ml-2 mr-2" v-if="child.post.id == selectedPostId"></span>
                    {{ child.post.since }}
                </small>
                <div class="wrapper">
                    <a :href="'/profil/' + child.post.user.id">
                        <strong>{{ child.post.user.nickName }}: </strong>
                    </a>
                    <span style="white-space: pre-wrap">{{ child.post.message }}</span>
                    <embeds :href="child.post.urlTipHref" />
                </div>
                <template v-if="child.children">
                    <children :children="child.children" :selected-post-id="selectedPostId" />
                </template>
            </div>
        </li>
    </ul>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'

import { Conversation } from '../../model/forum'
import { PageViewModel } from '../../model/common'

import Embeds from './embeds.vue'

@Component({
	props: {
        children: Array,
        selectedPostId: Number
	},
	components: {
		Embeds
	}
})
export default class Children extends Vue {
    children: Conversation[]
    selectedPostId: number
}
</script>

<style lang="scss" scoped>
.selected {
    border: 1px solid black;
}
ul.media-list {
    margin-top: 2rem !important;
}
.wrapper {
    margin-bottom: 2rem !important;
}
</style>
