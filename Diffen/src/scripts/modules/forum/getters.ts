import State from './state'
import { GetterTree } from 'vuex'

import { GET_PAGED_POSTS, GET_IS_LOADING_POSTS, GET_FILTER, GET_URLTIP_TOPLIST} from './types'

export const Getters: GetterTree<State, any>  = {
    [GET_PAGED_POSTS]: (state: State) => state.pagedPosts,
    [GET_IS_LOADING_POSTS]: (state: State) => state.isLoadingPosts,
    [GET_FILTER]: (state: State) => state.filter,
    [GET_URLTIP_TOPLIST]: (state: State) => state.urlTipTopList
}

export default Getters