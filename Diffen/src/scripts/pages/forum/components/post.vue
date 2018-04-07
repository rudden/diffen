<template>
    <li class="media list-group-item p-4">
        <a :href="'profile/' + post.user.id">
            <img class="media-object d-flex align-self-start mr-3" :src="post.user.avatar">
        </a>
        <div class="media-body">
            <div class="media-body-text">
                <div class="media-heading">
                    <small class="float-right text-muted">{{ post.since }}</small>
                    <h6>
                        {{ post.user.nickName }}
                        <template v-if="post.edited">
                            <span class="badge badge-danger ml-2">editerat</span>
                        </template>
                    </h6>
                </div>

                <template v-if="post.inEdit">
                    <new-post :post="post" />
                </template>
                <template v-else>
                    <p>{{ post.message }}</p>
                    <embeds :href="post.urlTipHref" />
                </template>
                <template v-if="post.inReply">
                    <div style="padding-top: 1rem">
                        <new-post :parent-id="post.id" />
                    </div>
                </template>

                <template v-if="showFooter">
                    <div class="message-footer">
                        <url v-bind="{ href: post.urlTipHref, text: 'länktips' }" v-if="post.urlTipHref" />
                        <span v-if="post.hasLineup && post.urlTipHref"> · </span>
                        <modal :id="modalId" v-bind="{ header: 'startelva', btnText: 'visa startelva' }" v-if="post.hasLineup">
                            <template slot="body">
                                <strong>asdasdasd</strong>
                            </template>
                        </modal>
                        <span v-if="(post.hasLineup || post.urlTipHref) && createdByLoggedInUser"> · </span>
                        <a v-on:click="post.inEdit = !post.inEdit" :class="{ 'disabled': post.inReply }" v-if="createdByLoggedInUser">
                            <span class="icon icon-edit"></span>
                        </a>
                        <span v-if="post.hasLineup || post.urlTipHref || createdByLoggedInUser"> · </span>
                        <a v-on:click="post.inReply = !post.inReply" :class="{ 'disabled': post.inEdit }">
                            <span class="icon icon-quote"></span>
                        </a>
                        <a v-on:click="bookmarkPost" v-if="canBookmark">
                             · <span class="icon icon-bookmark"></span>
                        </a>
                        <a v-on:click="scissorPost" v-if="loggeInUserIsAdmin">
                             · <span class="icon icon-scissors"></span>
                        </a>
                        <voting :post="post" />
                    </div>
                </template>

                <template v-if="post.parentPost">
                    <hr />
                    <ul class="media-list mt-3">
                        <li class="media">
                            <img class="media-object d-flex align-self-start mr-3" :src="post.parentPost.user.avatar">
                            <div class="media-body">
                                <div class="media-body-text">
                                    <div class="media-heading">
                                        <small class="float-right text-muted">{{ post.parentPost.since }}</small>
                                        <h6>{{ post.parentPost.user.nickName }}</h6>
                                    </div>
                                    <p>{{ post.parentPost.message }}</p>
                                </div>
                            </div>
                        </li>
                    </ul>
                </template>
            </div>
        </div>
    </li>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Action, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('forum', Getter)
const ModuleAction = namespace('forum', Action)

import { BOOKMARK_POST, SCISSOR_POST } from '../../../modules/forum/types'

import { Post, Vote, VoteType } from '../../../model/forum'
import { ViewModel } from '../../../model/common'

import NewPost from './new-post.vue'
import Url from '../../../components/url.vue'
import Embeds from '../../../components/embeds.vue'
import Modal from '../../../components/modal.vue'
import Voting from '../../../components/voting.vue'

@Component({
    props: {
        post: Object
    },
    components: {
        Url, Embeds, NewPost, Modal, Voting
    }
})
export default class PostComponent extends Vue {
    @State(state => state.vm) vm: ViewModel
    @ModuleAction(BOOKMARK_POST) bookmark: (payload: { postId: number }) => Promise<void>
    @ModuleAction(SCISSOR_POST) scissor: (payload: { postId: number }) => Promise<void>

    post: Post

    get loggeInUserIsAdmin(): boolean {
        return this.vm.loggedInUser.inRoles.includes('Sax' || 'Admin')
    }
    get createdByLoggedInUser(): boolean {
        return this.post.user.id == this.vm.loggedInUser.id ? true : false
    }
    get showFooter(): boolean {
        return this.post.hasLineup 
            || this.post.urlTipHref 
            || this.createdByLoggedInUser 
            || this.post.loggedInUserCanVote 
            || this.canBookmark 
            || this.loggeInUserIsAdmin 
            ? true : false
    }
    get modalId(): string {
        return `lineup-${this.post.id}`
    }
    get canBookmark(): boolean {
        return !this.vm.loggedInUser.savedPostsIds.includes(this.post.id)
    }

    bookmarkPost() {
        this.bookmark({ postId: this.post.id }).then(() => this.$forceUpdate())
    }
    scissorPost() {
        this.scissor({ postId: this.post.id }).then(() => this.$forceUpdate())
    }
}
</script>

<style lang="scss">
.media-body-text {
    p {
        margin-bottom: 0;
        white-space: pre-wrap;
    }
}
.message-footer {
    padding-top: 1rem;
    a {
        cursor: pointer;
        color: #3097D1 !important;
    }
    a.disabled {
        opacity: 0.4;
        pointer-events: none;
    }
}
</style>
