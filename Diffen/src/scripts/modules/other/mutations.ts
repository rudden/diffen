import State from './state'
import { MutationTree } from 'vuex'

import { Poll, Chronicle } from '../../model/other'

import { SET_POLLS, SET_CHRONICLES, SET_CHRONICLE } from './types'

export const Mutations: MutationTree<State> = {
    [SET_POLLS]: (state: State, polls: Poll[]) => { state.polls = polls },
    [SET_CHRONICLE]: (state: State, chronicle: Chronicle) => { state.chronicle = chronicle },
    [SET_CHRONICLES]: (state: State, chronicles: Chronicle[]) => { state.chronicles = chronicles }
}

export default Mutations