import State from './state'
import { GetterTree } from 'vuex'

import { GET_POLL, GET_POLLS, GET_CHRONICLES, GET_CHRONICLE, GET_REGIONS, GET_CHRONICLE_CATEGORIES } from './types'

export const Getters: GetterTree<State, any>  = {
    [GET_POLL]: (state: State) => state.poll,
    [GET_POLLS]: (state: State) => state.polls,
    [GET_CHRONICLE]: (state: State) => state.chronicle,
    [GET_CHRONICLES]: (state: State) => state.chronicles,
    [GET_CHRONICLE_CATEGORIES]: (state: State) => state.chronicleCategories,
    [GET_REGIONS]: (state: State) => state.regions
}

export default Getters