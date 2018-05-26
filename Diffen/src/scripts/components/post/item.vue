<template>
    <li class="media list-group-item p-4">
        <a :href="'/profil/' + post.user.id" class="large-device">
            <img class="media-object d-flex align-self-start mr-3" :src="post.user.avatar">
        </a>
        <div class="media-body">
            <small class="float-right text-muted">{{ post.since }}</small>
            <div class="media-heading">
                <a :href="`/profil/${post.user.id}`"><strong>{{ post.user.nickName }}</strong></a> 
                <span v-if="fullSize">
                    {{ post.inThreads.length > 0 ? 'skrev i ' : '' }}
                    <a href="#" v-for="thread in post.inThreads" v-tooltip="getThreadToolTip(thread.id)" :key="thread.id" @click="setThreadInFilter(thread.id)">
                        {{ `${thread.name}${post.inThreads.length > 0 && thread.id !== post.inThreads[post.inThreads.length - 1].id ? ', ' : ''}` }}
                    </a>
                </span>
            </div>
            <div class="media-body-text mt-2">
                <template v-if="post.parentPost && showParent && fullSize">
                    <div class="card mt-3 mb-3 parent-item">
                        <div class="card-body">
                            <div class="media">
                                <div class="media-body">
                                    <div class="media-body-text">
                                        <div class="media-heading">
                                            <small class="float-right text-muted">{{ post.parentPost.since }}</small>
                                            <h6>{{ post.parentPost.user.nickName }}</h6>
                                        </div>
                                        <span class="message more-readable" v-html="post.parentPost.message"></span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </template>
                <p v-html="postMessage" class="more-readable"></p>
                <embeds :href="post.urlTipHref" />
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
                            <template v-if="showActions">
                                <span v-if="(post.lineupId || post.urlTipHref) && (createdByLoggedInUser || (!createdByLoggedInUser && loggedInUserIsAdmin))"> · </span>
                                <modal v-bind="modalAttributes.editPost" v-if="createdByLoggedInUser">
                                    <template slot="body">
                                        <new-post :post="post" :modal-name="modalAttributes.editPost.attributes.name" v-bind="{ parentId: post.parentPost ? post.parentPost.id : null }" />
                                    </template>
                                </modal>
                                <modal v-bind="modalAttributes.threadsAdjuster" v-else-if="!createdByLoggedInUser && loggedInUserIsAdmin">
                                    <template slot="body">
                                        <threads-adjuster :post="post" />
                                    </template>
                                </modal>
                                <span v-if="post.lineupId || post.urlTipHref || createdByLoggedInUser || (!createdByLoggedInUser && loggedInUserIsAdmin)"> · </span>
                                <modal v-bind="modalAttributes.replyPost">
                                    <template slot="body">
                                        <div class="media-body">
                                            <div class="media-body-text">
                                                <post-main-content :post="post" />
                                            </div>
                                        </div>
                                        <hr />
                                        <new-post :parent-id="post.id" :modal-name="modalAttributes.replyPost.attributes.name" />
                                    </template>
                                    <template v-if="activeThread">
                                        <template slot="footer">
                                            <div class="col">
                                                Just nu pågår den planerade tråden <strong>{{ activeThread.name }}</strong>.
                                                <br />
                                                Ditt inlägg kommer automatiskt taggas i denna tråd.
                                            </div>
                                        </template>
                                    </template>
                                </modal>
                                <a v-on:click="bookmarkPost" v-if="canBookmark" v-tooltip="'Spara'">
                                    · <span class="icon icon-bookmark"></span>
                                </a>
                                <span v-if="(post.lineupId || post.urlTipHref || createdByLoggedInUser || canBookmark) && loggedInUserIsAdmin"> · </span>
                                <modal v-bind="modalAttributes.scissorPost" v-if="loggedInUserIsAdmin">
                                    <template slot="body">
                                        <div class="col pr-1 pl-1">
                                            <button class="btn btn-success btn-sm btn-block" v-on:click="scissorPost">Saxa!</button>
                                        </div>
                                    </template>
                                </modal>
                                <a :href="`/forum/inlagg/${post.id}`" class="no-hover" v-tooltip="'Gå till'">
                                    · <span class="icon icon-eye"></span>
                                </a>
                                <template v-if="post.updated">
                                    · <span class="badge badge-danger">editerat</span>
                                </template>
                                <voting :post="post" />
                            </template>
                        </div>
                    </template>
                </template>
                <template v-else>
                    <div class="message-footer">
                        <a :href="`/forum/inlagg/${post.id}`" class="no-hover" v-tooltip="'Gå till'">
                            <span class="icon icon-eye"></span>
                        </a>
                        <a v-on:click="unBookmarkPost" v-if="!canBookmark && showUnBookmarkBtn" v-tooltip="'Ta bort från sparade inlägg'">
                            · <span class="icon icon-trash"></span>
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
const ProfileModuleAction = namespace('profile', Action)

import {
    GET_ACTIVE_FIXED_THREAD,
    GET_FILTER,
    GET_PAGED_POSTS,
    GET_SHOULD_RELOAD_POST_STREAM,
    BOOKMARK_POST,
    SCISSOR_POST,
    FETCH_PAGED_POSTS,
    SET_IS_LOADING_POSTS,
    SET_SHOULD_RELOAD_POST_STREAM,
    SET_FILTER
} from '../../modules/forum/types'
import { FETCH_LINEUP } from '../../modules/squad/types'
import { UNBOOKMARK_POST } from '../../modules/profile/types'

import { Lineup } from '../../model/squad'
import { Post, Vote, VoteType, Filter, Thread } from '../../model/forum'
import { PageViewModel, Paging } from '../../model/common'

import NewPost from './new.vue'
import Url from '../url.vue'
import Embeds from './embeds.vue'
import Modal from '../modal.vue'
import Voting from './voting.vue'
import PostMainContent from './main-content.vue'
import ThreadsAdjuster from './threads-adjuster.vue'

import FormationComponent from '../lineups/formation.vue'

@Component({
    props: {
        post: Object,
        fullSize: {
            type: Boolean,
            default: true
        },
        showActions: {
            type: Boolean,
            default: true
        },
        showParent: {
            type: Boolean,
            default: true
        },
        showUnBookmarkBtn: {
			type: Boolean,
			default: false
		}
    },
    components: {
        Url, Embeds, NewPost, Modal, Voting, FormationComponent, PostMainContent, ThreadsAdjuster
    }
})
export default class PostComponent extends Vue {
    @State(state => state.vm) vm: PageViewModel
    @ModuleGetter(GET_ACTIVE_FIXED_THREAD) activeThread: Thread
    @ModuleGetter(GET_FILTER) filter: Filter
    @ModuleGetter(GET_PAGED_POSTS) pagedPosts: Paging<Post>
    @ModuleGetter(GET_SHOULD_RELOAD_POST_STREAM) shouldReloadPostStream: boolean
    @ModuleAction(BOOKMARK_POST) bookmark: (payload: { postId: number }) => Promise<void>
    @ModuleAction(SCISSOR_POST) scissor: (payload: { postId: number }) => Promise<void>
    @ModuleAction(FETCH_PAGED_POSTS) loadPaged: (payload: { pageNumber: number, pageSize: number, filter: Filter }) => Promise<void>
	@ModuleMutation(SET_IS_LOADING_POSTS) setIsLoadingPosts: (payload: { value: boolean }) => void
	@ModuleMutation(SET_SHOULD_RELOAD_POST_STREAM) setShouldReloadPostStream: (payload: { value: boolean }) => void
	@ModuleMutation(SET_FILTER) setFilter: (payload: { filter: Filter }) => void
    
    @SquadModuleAction(FETCH_LINEUP) loadLineup: (payload: { id: number }) => Promise<Lineup>

    @ProfileModuleAction(UNBOOKMARK_POST) unBookmark: (payload: { postId: number }) => Promise<void>

    post: Post
    fullSize: boolean
    showActions: boolean
    showParent: boolean
    showUnBookmarkBtn: boolean

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
                icon: 'icon icon-edit',
                text: 'Editera'
            },
            onOpen: () => this.post.inEdit = true,
            // onClose: this.closePostActionModal
        },
        scissorPost: {
            attributes: {
                name: `scissor-${this.post.id}`
            },
            header: 'Saxa inlägg',
            button: {
                icon: 'icon icon-scissors',
                text: 'Saxa'
            }
        },
        replyPost: {
            attributes: {
                name: `reply-${this.post.id}`,
                resizable: true,
                scrollable: true
            },
            header: 'Svara inlägg',
            button: {
                icon: 'icon icon-quote',
                text: 'Svara'
            },
            onOpen: () => this.post.inReply = true,
            // onClose: this.closePostActionModal
        },
        threadsAdjuster: {
            attributes: {
                name: `adjust-threads-${this.post.id}`,
                resizable: true,
                scrollable: true
            },
            header: 'Justera trådar på inlägget',
            button: {
                icon: 'icon icon-edit',
                text: 'Justera trådar'
            },
            onClose: this.reloadCurrentPage
        }
    }

    get loggedInUserIsAdmin(): boolean {
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
            || this.loggedInUserIsAdmin 
            ? true : false
    }
    get canBookmark(): boolean {
        return !this.vm.loggedInUser.savedPostsIds.includes(this.post.id)
    }

    get postMessage() {
        return this.filterMessage(this.post.message)
    }

    get parentPostMessage() {
        return this.filterMessage(this.post.parentPost.message)
    }

    filterMessage(message: string) {
        if (this.filter.messageWildCard && message) {
            var pattern = new RegExp(this.filter.messageWildCard, 'gi');
            return message.replace(pattern, '<strong><u>' + this.filter.messageWildCard + '</u></strong>')
        }
        return message
    }

    closePostActionModal() {
        if (this.post) {
            // reload all posts when modal is closed
            if (this.post.inEdit) {
                this.reloadCurrentPage()
                this.post.inEdit = false
            }
            if (this.post.inReply) {
                this.reloadPosts(1)
                this.post.inReply = false
            }
        }
    }

    reloadCurrentPage() {
        this.reloadPosts(this.pagedPosts.currentPage, true)
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
    unBookmarkPost() {
        this.unBookmark({ postId: this.post.id }).then(() => this.$forceUpdate())
    }
    scissorPost() {
        this.scissor({ postId: this.post.id }).then(() => this.$forceUpdate())
    }

    reloadPosts(pageNumber: number, override?: boolean) {
        if (this.shouldReloadPostStream || override) {
            this.setIsLoadingPosts({ value: true })
            this.loadPaged({ pageNumber: pageNumber, pageSize: this.vm.loggedInUser.filter.postsPerPage, filter: this.filter })
                .then(() => {
                    this.setIsLoadingPosts({ value: false })
                    this.setShouldReloadPostStream({ value: false })
                })
        }
    }

    getThreadToolTip(id: number) {
        if (this.filter.threadIds) {
            if (this.filter.threadIds.includes(id)) {
                return 'Ta bort denna tråd ur filtret'
            }
            if (this.filter.threadIds.length > 0) {
                return 'Lägg till tråd i filtret'
            } else {
                return 'Visa inlägg ur tråd'
            }
        }
    }

    setThreadInFilter(id: number) {
        let currentFilter = this.filter
        if (currentFilter.threadIds) {
            if (currentFilter.threadIds.length > 0) {
                if (!currentFilter.threadIds.includes(id)) {
                    currentFilter.threadIds.push(id)
                } else {
                    let index: number = currentFilter.threadIds.indexOf(id)
                    if (index > -1) {
                        currentFilter.threadIds.splice(index, 1)
                    }
                }
            } else {
                currentFilter.threadIds = [id]
            }
        }
        this.setFilter({ filter: currentFilter })
        this.setShouldReloadPostStream({ value: true })
        this.reloadPosts(1)
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
    a, .v-popover span {
        cursor: pointer;
        color: #3097D1 !important;
        .icon-thumbs-up, .icon-thumbs-down {
            color: #162248 !important;
        }
    }
    a.disabled {
        opacity: 0.4;
        pointer-events: none;
    }
}
.no-hover:hover {
    text-decoration: none;
}

.parent-item {
    background-color: #f5f8fa;
    .media-body-text span {
        white-space: pre-wrap;
    }
}
</style>
