import State from './state'
import { GetterTree } from 'vuex'

import {
    GET_FORMATIONS,
    GET_LINEUPS,
    GET_NEW_LINEUP,
    GET_PLAYERS,
    GET_POSITIONS,
    GET_SELECTED_LINEUP
} from './types'

export const Getters: GetterTree<State, any>  = {
    [GET_FORMATIONS]: (state: State) => state.formations,
    [GET_LINEUPS]: (state: State) => state.lineups,
    [GET_NEW_LINEUP]: (state: State) => state.newLineup,
    [GET_PLAYERS]: (state: State) => state.players,
    [GET_POSITIONS]: (state: State) => state.positions,
    [GET_SELECTED_LINEUP]: (state: State) => state.selectedLineup
}

export default Getters