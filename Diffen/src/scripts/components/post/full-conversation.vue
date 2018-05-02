<template>
	<ul class="media-list media-list-stream">
        <li class="list-group-item p-4">
            <a href="/forum" class="btn btn-sm btn-primary float-right">tillbaka till forumet</a>
            <h4 class="mb-0">konversation</h4>
        </li>
        <li class="media list-group-item p-4" v-show="loading">
			<loader v-bind="{ background: '#699ED0' }" />
		</li>
        <li class="media list-group-item p-4" v-show="!loading">
            <template v-if="!loading && !nothingFound">
                <a :href="'/profil/' + conversation.post.user.id">
                    <img class="media-object mr-3 align-self-start" :src="conversation.post.user.avatar">
                </a>
                <div class="media-body">
                    <div class="media-heading">
                        <small class="float-right text-muted">
                            <span class="icon icon-check ml-2 mr-2" v-if="conversation.post.id == selectedPostId"></span>
                            {{ conversation.post.since }}
                        </small>
                        <h6>{{ conversation.post.user.nickName }}</h6>
                    </div>
                    <p style="white-space: pre-wrap">{{ conversation.post.message }}</p>
                    <embeds :href="conversation.post.urlTipHref" />
                    <template v-if="conversation.children">
                        <children :children="conversation.children" :selected-post-id="selectedPostId" />
                    </template>
                </div>
            </template>
            <template v-if="nothingFound">
                <div class="alert alert-warning mb-0" style="display: block; width: 100%">hittade ingenting</div>
            </template>
        </li>
	</ul>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Action, State, namespace } from 'vuex-class'

const ModuleAction = namespace('forum', Action)

import { FETCH_CONVERSATION_ON_POST } from '../../modules/forum/types'

import { PageViewModel } from '../../model/common'
import { Post, Conversation } from '../../model/forum'

import Embeds from './embeds.vue'
import Children from './children.vue'

@Component({
	props: {
        selectedPostId: Number
	},
	components: {
		Children, Embeds
	}
})
export default class FullConversation extends Vue {
    @State(state => state.vm) vm: PageViewModel
    @ModuleAction(FETCH_CONVERSATION_ON_POST) loadConversation: (payload: { postId: number }) => Promise<Post[]>

    selectedPostId: number

    loading: boolean = true
    posts: Post[] = []

    nothingFound: boolean = false

    mounted() {
        this.loadConversation({ postId: this.selectedPostId })
            .then((posts: Post[]) => {
                if (posts.length > 0)
                    this.posts = posts
                else
                    this.nothingFound = true
                this.loading = false
            })
    }
    
    get conversation() {
        let children = this.getChildren(this.posts[0])
        if (children) {
            let conversation: Conversation = {
                post: this.posts[0],
                children: children
            }
            this.filterConversations(conversation)
            return conversation
        }
    }

    getChildren(post: Post): Conversation[] {
        let children = this.getSubChildren(post.id)
        return children.length > 0 ? this.getConversations(children) : []
    }

    getSubChildren(postId: number) {
        return this.posts.filter((post: Post) => post.parentPost ? post.parentPost.id == postId : false)
    }

    getConversations(posts: Post[]) {
        let arrayOfChildren: Conversation[] = []
        posts.forEach((p: Post) => {
            arrayOfChildren.push({
                post: p,
                children: this.getChildren(p)
            })
        })
        return arrayOfChildren.length > 0 ? arrayOfChildren : []
    }

    filterConversations(conversation: Conversation) {
        if (conversation.children.length == 0) {
            delete conversation.children
            return
        }
        conversation.children.forEach((childConversation: Conversation) => {
            this.filterConversations(childConversation)
        })
    }
}
</script>
