import State from './state'
import { GetterTree } from 'vuex'

import { GET_THREADS, GET_PAGED_POSTS, GET_IS_LOADING_POSTS, GET_FILTER, GET_URLTIP_TOPLIST, GET_SHOW_LEFT_SIDEBAR, GET_SHOW_RIGHT_SIDEBAR, GET_SHOULD_RELOAD_POST_STREAM, GET_SELECTED_CONVERSATION, GET_ACTIVE_FIXED_THREAD} from './types'

export const Getters: GetterTree<State, any>  = {
    [GET_PAGED_POSTS]: (state: State) => state.pagedPosts,
    [GET_IS_LOADING_POSTS]: (state: State) => state.isLoadingPosts,
    [GET_FILTER]: (state: State) => state.filter,
    [GET_URLTIP_TOPLIST]: (state: State) => state.urlTipTopList,
    [GET_SHOW_LEFT_SIDEBAR]: (state: State) => state.showLeftSideBar,
    [GET_SHOW_RIGHT_SIDEBAR]: (state: State) => state.showRightSideBar,
    [GET_SHOULD_RELOAD_POST_STREAM]: (state: State) => state.shouldReloadPostStream,
    [GET_SELECTED_CONVERSATION]: (state: State) => state.selectedConversation,
    [GET_THREADS]: (state: State) => state.threads,
    [GET_ACTIVE_FIXED_THREAD]: (state: State) => state.activeFixedThread,
}

export default Getters