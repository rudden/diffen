import State from './state'
import { MutationTree } from 'vuex'

import { User } from '../../model/profile'
import { KeyValuePair } from '../../model/common'

import { 
    SET_USER,
    SET_KVP_USERS
} from './types'

export const Mutations: MutationTree<State> = {
    [SET_USER]: (state: State, user: User) => { state.user = user },
    [SET_KVP_USERS]: (state: State, users: KeyValuePair[]) => { state.users = users },
}

export default Mutations