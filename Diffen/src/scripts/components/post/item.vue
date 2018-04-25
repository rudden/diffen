<template>
    <li class="media list-group-item p-4">
        <a :href="'/profile/' + post.user.id">
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
                            <modal v-bind="{ id: `lineup-${post.id}`, header: 'startelva' }" v-if="post.lineupId">
                                <template slot="btn">
									 <small><a href="#" data-toggle="modal" :data-target="'#' + `lineup-${post.id}`" v-on:click="getLineup">visa startelva</a></small>
								</template>
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
                            <modal v-bind="{ id: `edit-${post.id}`, header: 'editera inlägg' }" v-if="createdByLoggedInUser" :close-action="closePostActionModal">
                                <template slot="btn">
                                    <a data-toggle="modal" :data-target="'#' + `edit-${post.id}`" v-on:click="post.inEdit = true">
                                        <span class="icon icon-edit"></span>
                                    </a>
								</template>
                                <template slot="body" v-if="post.inEdit">
                                    <new-post :post="post" v-bind="{ parentId: post.parentPost ? post.parentPost.id : null }" />
                                </template>
                            </modal>
                            <span v-if="post.lineupId || post.urlTipHref || createdByLoggedInUser"> · </span>
                            <modal v-bind="{ id: `reply-${post.id}`, header: 'svara inlägg' }" :close-action="closePostActionModal">
                                <template slot="btn">
                                    <a data-toggle="modal" :data-target="'#' + `reply-${post.id}`" v-on:click="post.inReply = true">
                                        <span class="icon icon-quote"></span>
                                    </a>
								</template>
                                <template slot="body" v-if="post.inReply">
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
                                · <span class="icon icon-mask"></span>
                            </a>
                            <voting :post="post" />
                        </div>
                    </template>

                    <template v-if="post.parentPost">
                        <hr />
                        <ul class="media-list mt-3">
                            <li class="media">
                                <a :href="'/profile/' + post.parentPost.user.id">
                                    <img class="media-object d-flex align-self-start mr-3" :src="post.parentPost.user.avatar">
                                </a>
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
                </template>
                <template v-else>
                    <div class="message-footer">
                        <a :href="`/forum/post/${post.id}`" class="no-hover">
                            <span class="icon icon-mask"></span>
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

import { GET_FILTER, BOOKMARK_POST, SCISSOR_POST, FETCH_PAGED_POSTS, SET_IS_LOADING_POSTS } from '../../modules/forum/types'
import { FETCH_LINEUP } from '../../modules/squad/types'

import { Lineup } from '../../model/squad'
import { Post, Vote, VoteType, Filter } from '../../model/forum'
import { ViewModel, Paging } from '../../model/common'

import NewPost from './new.vue'
import Url from '../url.vue'
import Embeds from './embeds.vue'
import Modal from '../modal.vue'
import Voting from './voting.vue'
import PostMainContent from './main-content.vue'

import FormationComponent from '../lineups/formation.vue'

import { Stretch as Loader } from 'vue-loading-spinner'

@Component({
    props: {
        post: Object,
        fullSize: {
            type: Boolean,
            default: true
        }
    },
    components: {
        Url, Embeds, NewPost, Modal, Voting, FormationComponent, Loader, PostMainContent
    }
})
export default class PostComponent extends Vue {
    @State(state => state.vm) vm: ViewModel
    @ModuleGetter(GET_FILTER) filter: Filter
    @ModuleAction(BOOKMARK_POST) bookmark: (payload: { postId: number }) => Promise<void>
    @ModuleAction(SCISSOR_POST) scissor: (payload: { postId: number }) => Promise<void>
    @ModuleAction(FETCH_PAGED_POSTS) loadPaged: (payload: { pageNumber: number, pageSize: number, filter: Filter }) => Promise<void>
	@ModuleMutation(SET_IS_LOADING_POSTS) setIsLoadingPosts: (payload: { value: boolean }) => void
    
    @SquadModuleAction(FETCH_LINEUP) loadLineup: (payload: { id: number }) => Promise<Lineup>

    post: Post
    fullSize: boolean
    
    lineup: Lineup = new Lineup()
    loadingLineup: boolean = false

    get loggeInUserIsAdmin(): boolean {
        return this.vm.loggedInUser.inRoles.includes('Sax' || 'Admin')
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

    created() {
        // listen for escape key events
        // ugly solution since i'm not able to access jquery in vue sfc written in ts
        window.addEventListener('keyup', (e) => {
            if (e.which == 27) {
                this.closePostActionModal()
            }
        })
    }

    closePostActionModal() {
        if (this.post) {
            // reload all posts when modal is closed
            if (this.post.inEdit || this.post.inReply) {
                this.reloadPosts()
            }
            if (this.post.inEdit) {
                this.post.inEdit = false
            }
            if (this.post.inReply) {
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

    reloadPosts() {
        this.setIsLoadingPosts({ value: true })
        this.loadPaged({ pageNumber: 1, pageSize: this.vm.loggedInUser.filter.postsPerPage, filter: this.filter })
            .then(() => this.setIsLoadingPosts({ value: false }))
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
