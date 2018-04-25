import State from './state'
import { GetterTree } from 'vuex'

import { GET_POLLS, GET_CHRONICLES, GET_CHRONICLE } from './types'

export const Getters: GetterTree<State, any>  = {
    [GET_POLLS]: (state: State) => state.polls,
    [GET_CHRONICLE]: (state: State) => state.chronicle,
    [GET_CHRONICLES]: (state: State) => state.chronicles
}

export default Getters