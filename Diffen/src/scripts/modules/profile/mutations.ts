import State from './state'
import { MutationTree } from 'vuex'

import { User } from '../../model/profile'
import { Paging } from '../../model/common'
import { Post } from '../../model/forum'

import { 
    SET_USER,
    SET_CREATED_POSTS,
    SET_SAVED_POSTS
} from './types'

export const Mutations: MutationTree<State> = {
    [SET_USER]: (state: State, user: User) => { state.user = user },
    [SET_CREATED_POSTS]: (state: State, posts: Paging<Post>) => { state.createdPosts = posts },
    [SET_SAVED_POSTS]: (state: State, posts: Paging<Post>) => { state.savedPosts = posts }
}

export default Mutations