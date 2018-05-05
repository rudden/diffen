<template>
    <li class="media list-group-item p-4">
        <a :href="'/profil/' + post.user.id">
            <img class="media-object d-flex align-self-start mr-3" :src="post.user.avatar">
        </a>
        <div class="media-body">
            <div class="media-body-text">
                <post-main-content :post="post" />
                <!-- show actions and conversation on condition -->
                <template v-if="fullSize">
                    <template v-if="showFooter">
                        <div class="message-footer">
                            <url v-bind="{ href: post.urlTipHref, text: 'länktips', postId: post.id }" v-if="post.urlTipHref" />
                            <span v-if="post.lineupId && post.urlTipHref"> · </span>
                            <modal v-bind="modalAttributes.startingEleven" v-if="post.lineupId">
                                <template slot="body">
                                    <template v-if="lineup.id && !loadingLineup">
                                        <formation-component :formation="lineup.formation" :players="lineup.players" />
                                    </template>
                                    <template v-else>
                                        <loader v-bind="{ background: '#699ED0' }" />
                                    </template>
                                </template>
                            </modal>
                            <span v-if="(post.lineupId || post.urlTipHref) && createdByLoggedInUser"> · </span>
                            <modal v-bind="modalAttributes.editPost" v-if="createdByLoggedInUser">
                               <template slot="body">
                                    <new-post :post="post" v-bind="{ parentId: post.parentPost ? post.parentPost.id : null }" />
                                </template>
                            </modal>
                            <span v-if="post.lineupId || post.urlTipHref || createdByLoggedInUser"> · </span>
                            <modal v-bind="modalAttributes.replyPost">
                                <template slot="body">
                                    <div class="media-body">
                                        <div class="media-body-text">
                                            <post-main-content :post="post" />
                                        </div>
                                    </div>
                                    <hr />
                                    <new-post :parent-id="post.id" />
                                </template>
                            </modal>
                            <a v-on:click="bookmarkPost" v-if="canBookmark">
                                · <span class="icon icon-bookmark"></span>
                            </a>
                            <a v-on:click="scissorPost" v-if="loggeInUserIsAdmin">
                                · <span class="icon icon-scissors"></span>
                            </a>
                            <a :href="`/forum/post/${post.id}`" class="no-hover">
                                · <span class="icon icon-eye"></span>
                            </a>
                            <voting :post="post" />
                        </div>
                    </template>

                    <template v-if="post.parentPost">
                        <hr />
                        <ul class="media-list mt-3">
                            <li class="media">
                                <a :href="'/profil/' + post.parentPost.user.id">
                                    <img class="media-object d-flex align-self-start mr-3" :src="post.parentPost.user.avatar">
                                </a>
                                <div class="media-body">
                                    <div class="media-body-text">
                                        <div class="media-heading">
                                            <small class="float-right text-muted">{{ post.parentPost.since }}</small>
                                            <a :href="`/profil/${post.user.id}`">
                                                <h6>{{ post.parentPost.user.nickName }}</h6>
                                            </a>
                                        </div>
                                        <p>{{ post.parentPost.message }}</p>
                                    </div>
                                </div>
                            </li>
                        </ul>
                    </template>
                </template>
                <template v-else>
                    <div class="message-footer">
                        <a :href="`/forum/post/${post.id}`" class="no-hover">
                            <span class="icon icon-eye"></span>
                        </a>
                    </div>
                </template>
            </div>
        </div>
    </li>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Action, Mutation, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('forum', Getter)
const ModuleAction = namespace('forum', Action)
const ModuleMutation = namespace('forum', Mutation)
const SquadModuleGetter = namespace('squad', Getter)
const SquadModuleAction = namespace('squad', Action)

import {
    GET_FILTER,
    GET_PAGED_POSTS,
    GET_SHOULD_RELOAD_POST_STREAM,
    BOOKMARK_POST,
    SCISSOR_POST,
    FETCH_PAGED_POSTS,
    SET_IS_LOADING_POSTS,
    SET_SHOULD_RELOAD_POST_STREAM
} from '../../modules/forum/types'
import { FETCH_LINEUP } from '../../modules/squad/types'

import { Lineup } from '../../model/squad'
import { Post, Vote, VoteType, Filter } from '../../model/forum'
import { PageViewModel, Paging } from '../../model/common'

import NewPost from './new.vue'
import Url from '../url.vue'
import Embeds from './embeds.vue'
import Modal from '../modal.vue'
import Voting from './voting.vue'
import PostMainContent from './main-content.vue'

import FormationComponent from '../lineups/formation.vue'

@Component({
    props: {
        post: Object,
        fullSize: {
            type: Boolean,
            default: true
        }
    },
    components: {
        Url, Embeds, NewPost, Modal, Voting, FormationComponent, PostMainContent
    }
})
export default class PostComponent extends Vue {
    @State(state => state.vm) vm: PageViewModel
    @ModuleGetter(GET_FILTER) filter: Filter
    @ModuleGetter(GET_PAGED_POSTS) pagedPosts: Paging<Post>
    @ModuleGetter(GET_SHOULD_RELOAD_POST_STREAM) shouldReloadPostStream: boolean
    @ModuleAction(BOOKMARK_POST) bookmark: (payload: { postId: number }) => Promise<void>
    @ModuleAction(SCISSOR_POST) scissor: (payload: { postId: number }) => Promise<void>
    @ModuleAction(FETCH_PAGED_POSTS) loadPaged: (payload: { pageNumber: number, pageSize: number, filter: Filter }) => Promise<void>
	@ModuleMutation(SET_IS_LOADING_POSTS) setIsLoadingPosts: (payload: { value: boolean }) => void
	@ModuleMutation(SET_SHOULD_RELOAD_POST_STREAM) setShouldReloadPostStream: (payload: { value: boolean }) => void
    
    @SquadModuleAction(FETCH_LINEUP) loadLineup: (payload: { id: number }) => Promise<Lineup>

    post: Post
    fullSize: boolean
    
    lineup: Lineup = new Lineup()
    loadingLineup: boolean = false

    modalAttributes: any = {
        startingEleven: {
            attributes: {
                name: `lineup-${this.post.id}`,
                scrollable: true
            },
            button: {
                classes: 'small',
                text: 'visa startelva'
            },
            onOpen: this.getLineup
        },
        editPost: {
            attributes: {
                name: `edit-${this.post.id}`,
                scrollable: true
            },
            header: 'Editera inlägg',
            button: {
                icon: 'icon icon-edit'
            },
            onOpen: () => this.post.inEdit = true,
            onClose: this.closePostActionModal
        },
        replyPost: {
            attributes: {
                name: `reply-${this.post.id}`,
                resizable: true,
                scrollable: true
            },
            header: 'Svara inlägg',
            button: {
                icon: 'icon icon-quote'
            },
            onOpen: () => this.post.inReply = true,
            onClose: this.closePostActionModal
        }
    }

    get loggeInUserIsAdmin(): boolean {
        return this.vm.loggedInUser.inRoles.some(role => role == 'Scissor' || role == 'Admin')
    }
    get createdByLoggedInUser(): boolean {
        return this.post.user.id == this.vm.loggedInUser.id ? true : false
    }
    get showFooter(): boolean {
        return this.post.lineupId 
            || this.post.urlTipHref 
            || this.createdByLoggedInUser 
            || this.post.loggedInUserCanVote 
            || this.canBookmark 
            || this.loggeInUserIsAdmin 
            ? true : false
    }
    get canBookmark(): boolean {
        return !this.vm.loggedInUser.savedPostsIds.includes(this.post.id)
    }

    closePostActionModal() {
        if (this.post) {
            // reload all posts when modal is closed
            if (this.post.inEdit) {
                this.reloadPosts(this.pagedPosts.currentPage)
                this.post.inEdit = false
            }
            if (this.post.inReply) {
                this.reloadPosts(1)
                this.post.inReply = false
            }
        }
    }

    getLineup() {
        this.loadingLineup = true
        this.loadLineup({ id: <number>this.post.lineupId })
            .then((lineup: Lineup) => {
                this.lineup = lineup
                this.loadingLineup = false
            })
    }
    bookmarkPost() {
        this.bookmark({ postId: this.post.id }).then(() => this.$forceUpdate())
    }
    scissorPost() {
        this.scissor({ postId: this.post.id }).then(() => this.$forceUpdate())
    }

    reloadPosts(pageNumber: number) {
        if (this.shouldReloadPostStream) {
            this.setIsLoadingPosts({ value: true })
            this.loadPaged({ pageNumber: pageNumber, pageSize: this.vm.loggedInUser.filter.postsPerPage, filter: this.filter })
                .then(() => {
                    this.setIsLoadingPosts({ value: false })
                    this.setShouldReloadPostStream({ value: false })
                })
        }
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
.no-hover:hover {
    text-decoration: none;
}
</style>
