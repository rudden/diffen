import State from './state'
import { MutationTree } from 'vuex'

import { Post, VoteType, Filter, UrlTip, Conversation, Thread } from '../../model/forum'
import { Vote as CrudVote } from '../../model/forum/crud'
import { Paging, KeyValuePair } from '../../model/common'

import { 
    SET_PAGED_POSTS,
    SET_POST_AFTER_VOTE,
    SET_IS_LOADING_POSTS,
    SET_REMOVE_POST_FROM_LIST,
    SET_FILTER,
    SET_URLTIP_TOPLIST,
    SET_SHOW_LEFT_SIDEBAR,
    SET_SHOW_RIGHT_SIDEBAR,
    SET_SHOULD_RELOAD_POST_STREAM,
    SET_SELECTED_CONVERSATION,
    SET_THREADS
} from './types'

export const Mutations: MutationTree<State> = {
    [SET_PAGED_POSTS]: (state: State, payload: { pagedPosts: Paging<Post>, concat: boolean }) => { 
        state.pagedPosts = {
            data: payload.concat ? state.pagedPosts.data.concat(payload.pagedPosts.data) : payload.pagedPosts.data,
            currentPage: payload.pagedPosts.currentPage,
            numberOfPages: payload.pagedPosts.numberOfPages,
            total: payload.pagedPosts.total
        }
    },
    [SET_POST_AFTER_VOTE]: (state: State, payload: { vote: CrudVote, nickName: string }) => { 
        for (let i = 0; i < state.pagedPosts.data.length; i++) {
            if (state.pagedPosts.data[i].id !== payload.vote.postId)
                continue
            state.pagedPosts.data[i].votes.push({
                type: payload.vote.type,
                byNickName: payload.nickName
            })
            state.pagedPosts.data[i].loggedInUserCanVote = false
        }
    },
    [SET_IS_LOADING_POSTS]: (state: State, payload: { value: boolean }) => { 
        state.isLoadingPosts = payload.value
    },
    [SET_REMOVE_POST_FROM_LIST]: (state: State, postId: number) => { 
        state.pagedPosts.data = state.pagedPosts.data.filter((p: Post) => p.id !== postId)
    },
    [SET_FILTER]: (state: State, payload: { filter: Filter }) => { 
        state.filter = payload.filter
    },
    [SET_URLTIP_TOPLIST]: (state: State, urlTips: UrlTip[]) => { state.urlTipTopList = urlTips },
    [SET_SHOW_LEFT_SIDEBAR]: (state: State, payload: { value: boolean }) => { state.showLeftSideBar = payload.value },
    [SET_SHOW_RIGHT_SIDEBAR]: (state: State, payload: { value: boolean }) => { state.showRightSideBar = payload.value },
    [SET_SHOULD_RELOAD_POST_STREAM]: (state: State, payload: { value: boolean }) => { state.shouldReloadPostStream = payload.value },
    [SET_SELECTED_CONVERSATION]: (state: State, posts: Post[]) => {
        state.selectedConversation = fullConversation(posts)

        function fullConversation(posts: Post[]): Conversation {
            let basePost = posts[0]
            let children = getChildren(posts, basePost.id)
            if (children) {
                let conversation: Conversation = {
                    post: basePost,
                    children: children,
                    all: posts
                }
                filterConversations(conversation)
                return conversation
            } else {
                return new Conversation()
            }
        }
        
        function getChildren(posts: Post[], postId: number) {
            let children = posts.filter((post: Post) => post.parentPost ? post.parentPost.id == postId : false)
            return children.length > 0 ? getConversations(posts, children) : []
        }
        
        function getConversations(posts: Post[], children: Post[]) {
            let arrayOfChildren: Conversation[] = []
            children.forEach((post: Post) => {
                arrayOfChildren.push({
                    post: post,
                    children: getChildren(posts, post.id)
                })
            })
            return arrayOfChildren.length > 0 ? arrayOfChildren : []
        }
        
        function filterConversations(conversation: Conversation) {
            if (conversation.children.length == 0) {
                delete conversation.children
                if (!conversation.all) {
                    delete conversation.all
                }
                return
            }
            conversation.children.forEach((childConversation: Conversation) => {
                filterConversations(childConversation)
            })
        }
    },
    [SET_THREADS]: (state: State, threads: Thread[]) => { 
        state.threads = threads
    },
}

export default Mutations