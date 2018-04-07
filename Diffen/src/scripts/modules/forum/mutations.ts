import State from './state'
import { MutationTree } from 'vuex'

import { Post, VoteType, Filter } from '../../model/forum'
import { Vote as CrudVote } from '../../model/forum/crud'
import { Paging, KeyValuePair } from '../../model/common'

import { 
    SET_PAGED_POSTS,
    SET_POST_AFTER_VOTE,
    SET_IS_LOADING_POSTS,
    SET_REMOVE_POST_FROM_LIST,
    SET_FILTER,
    SET_KVP_USERS
} from './types'

export const Mutations: MutationTree<State> = {
    [SET_PAGED_POSTS]: (state: State, pagedPosts: Paging<Post>) => { 
        state.pagedPosts = pagedPosts 
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
        let index: number = 0
        for (let i = 0; i < state.pagedPosts.data.length; i++) {
            if (state.pagedPosts.data[i].id !== postId)
                continue
            index = state.pagedPosts.data.indexOf(state.pagedPosts.data[i])
            break
        }
        if (index > -1)
		{
			state.pagedPosts.data.splice(index, 1)
		}
    },
    [SET_FILTER]: (state: State, payload: { filter: Filter }) => { 
        state.filter = payload.filter
    },
    [SET_KVP_USERS]: (state: State, users: KeyValuePair[]) => { 
        state.users = users
    },
}

export default Mutations