<template>
	<ul class="media-list media-list-stream">
        <li class="list-group-item p-4">
            <a href="/forum" class="btn btn-sm btn-primary float-right">Tillbaka till forumet</a>
            <h4 class="mb-0">{{ hasSelectedSinglePost ? 'Inlägg' : 'Konversation' }}</h4>
        </li>
        <template v-if="loading">
            <li class="media list-group-item p-4">
                <loader v-bind="{ background: '#699ED0' }" />
            </li>    
        </template>
        <template v-else>
            <template v-if="nothingFound">
                <div class="alert alert-warning mb-0" style="display: block; width: 100%">Hittade ingenting</div>
            </template>
            <template v-else>
                <template v-if="!hasSelectedSinglePost">
                    <li class="media list-group-item p-4">
                        <a :href="'/profil/' + selectedConversation.post.user.id">
                            <img class="media-object mr-3 align-self-start" :src="selectedConversation.post.user.avatar">
                        </a>
                        <div class="media-body">
                            <div class="media-heading">
                                <small class="float-right text-muted">
                                    <span class="icon icon-check ml-2 mr-2" v-if="selectedConversation.post.id == selectedPostId"></span>
                                    {{ selectedConversation.post.since }}
                                </small>
                                <a :href="'/profil/' + selectedConversation.post.user.id">
                                    <h6>{{ selectedConversation.post.user.nickName }}</h6>
                                </a>
                            </div>
                            <p style="white-space: pre-wrap">{{ selectedConversation.post.message }}</p>
                            <p class="mb-0">
                                <a :href="`/forum/inlagg/${selectedConversation.post.id}`" v-on:click="setAsActive(selectedConversation.post.id)">
                                    <span class="icon icon-link"></span>
                                </a>
                            </p>
                            <embeds :href="selectedConversation.post.urlTipHref" />
                            <template v-if="selectedConversation.children">
                                <children :children="selectedConversation.children" :selected-post-id="selectedPostId" />
                            </template>
                        </div>
                    </li>
                </template>
                <template v-else>
                    <post-component :post="selectedPost" :show-parent="false" :show-actions="false" />
                    <li class="media list-group-item p-4" v-if="selectedConversation.all.length > 1">
                        <div class="btn-group">
                            <a :href="`/forum/inlagg/${selectedPostId}/konversation`" class="btn btn-sm btn-primary">Visa hela konversationen</a>
                        </div>
                    </li>
                </template>
            </template>
        </template>
	</ul>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Action, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('forum', Getter)
const ModuleAction = namespace('forum', Action)

import { GET_SELECTED_CONVERSATION, FETCH_POST, FETCH_CONVERSATION_ON_POST } from '../../modules/forum/types'

import { ForumViewModel } from '../../model/common'
import { Post, Conversation } from '../../model/forum'

import PostComponent from './item.vue'
import Embeds from './embeds.vue'
import Children from './children.vue'

@Component({
	props: {
        selectedPostId: Number
	},
	components: {
		PostComponent, Children, Embeds
	}
})
export default class StandalonePost extends Vue {
    @State(state => state.vm) vm: ForumViewModel
    @ModuleGetter(GET_SELECTED_CONVERSATION) selectedConversation: Conversation
    @ModuleAction(FETCH_POST) loadPost: (payload: { postId: number }) => Promise<Post>
    @ModuleAction(FETCH_CONVERSATION_ON_POST) loadConversation: (payload: { postId: number }) => Promise<void>

    selectedPostId: number
    loading: boolean = true
    nothingFound: boolean = false

    selectedPost: Post = new Post()

    mounted() {
        this.loadConversation({ postId: this.selectedPostId })
            .then(() => {
                if (!this.vm.fullConversationMode) {
                    this.setAsActive(this.selectedPostId)
                }
                this.loading = false
            })
    }

    get hasSelectedSinglePost() {
        return this.selectedPost.id > 0 ? true : false
    }

    setAsActive(postId: number) {
        if (this.selectedConversation.all) {
            this.selectedPost = this.selectedConversation.all.filter((post: Post) => post.id == postId)[0]
        }
    }
}
</script>