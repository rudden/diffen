import State from './state'
import { GetterTree } from 'vuex'

import {
    GET_USER,
    GET_CREATED_POSTS,
    GET_SAVED_POSTS
} from './types'

export const Getters: GetterTree<State, any>  = {
    [GET_USER]: (state: State) => state.user,
    [GET_CREATED_POSTS]: (state: State) => state.createdPosts,
    [GET_SAVED_POSTS]: (state: State) => state.savedPosts
}

export default Getters