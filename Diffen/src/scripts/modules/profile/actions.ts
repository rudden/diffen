import axios from 'axios'
import State from './state'
import { Store, ActionTree, ActionContext } from 'vuex'

import { User } from '../../model/profile'
// import { Result } from '../../model/common'

import { 
	FETCH_USER,
	FETCH_KVP_USERS,
	SET_USER,
    SET_KVP_USERS
} from './types'

// export everything compliant to the vuex specification for actions
export const Actions: ActionTree<State, any> = {
	[FETCH_USER]: (store: ActionContext<State, any>, payload: { id: string }): Promise<void> => {
		return axios.get(`${store.rootState.vm.api}/users/${payload.id}`)
			.then((res) => store.commit(SET_USER, res.data)).catch((error) => console.warn(error))
	},
	[FETCH_KVP_USERS]: (store: ActionContext<State, any>): Promise<void> => {
		return axios.get(`${store.rootState.vm.api}/users`)
			.then((res) => store.commit(SET_KVP_USERS, res.data)).catch((error) => console.warn(error))
	},
}
export default Actions