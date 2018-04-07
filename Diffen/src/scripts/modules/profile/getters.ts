import State from './state'
import { GetterTree } from 'vuex'

import { GET_USER, GET_KVP_USERS } from './types'

export const Getters: GetterTree<State, any>  = {
    [GET_USER]: (state: State) => state.user,
    [GET_KVP_USERS]: (state: State) => state.users
}

export default Getters