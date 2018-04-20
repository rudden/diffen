import axios from 'axios'
import State from './state'
import { Store, ActionTree, ActionContext } from 'vuex'

import { Result, KeyValuePair } from '../../model/common'
import { Lineup, Player, Formation } from '../../model/squad'
import { Lineup as CrudLineup, Player as CrudPlayer } from '../../model/squad/crud'

import { 
	FETCH_KVP_USERS,
	FETCH_LINEUPS_ON_USER,
	FETCH_FORMATIONS,
	FETCH_PLAYERS,
	FETCH_POSITIONS,
	CREATE_LINEUP,
	FETCH_LINEUP_ON_POST,
	FETCH_LINEUP,
	SET_LINEUPS,
	SET_PLAYERS,
	SET_FORMATIONS,
	SET_SELECTED_LINEUP,
	SET_POSITIONS,
    UPDATE_PLAYER,
	CREATE_PLAYER
} from './types'

// export everything compliant to the vuex specification for actions
export const Actions: ActionTree<State, any> = {
	[FETCH_KVP_USERS]: (store: ActionContext<State, any>): Promise<KeyValuePair[]> => {
		return new Promise<KeyValuePair[]>((resolve, reject) => {
			return axios.get(`${store.rootState.vm.api}/users`)
				.then((res) => resolve(res.data)).catch((error) => console.warn(error))
		})
	},
	[FETCH_LINEUPS_ON_USER]: (store: ActionContext<State, any>, payload: { userId: string }): Promise<void> => {
		return axios.get(`${store.rootState.vm.api}/squads/lineups/user/${payload.userId}`)
			.then((res) => store.commit(SET_LINEUPS, res.data)).catch((error) => console.warn(error))
	},
	[FETCH_FORMATIONS]: (store: ActionContext<State, any>): Promise<void> => {
		return axios.get(`${store.rootState.vm.api}/squads/formations`)
			.then((res) => store.commit(SET_FORMATIONS, res.data)).catch((error) => console.warn(error))
	},
	[FETCH_PLAYERS]: (store: ActionContext<State, any>): Promise<void> => {
		return axios.get(`${store.rootState.vm.api}/squads/players`)
			.then((res) => store.commit(SET_PLAYERS, res.data)).catch((error) => console.warn(error))
	},
	[FETCH_POSITIONS]: (store: ActionContext<State, any>): Promise<void> => {
		return axios.get(`${store.rootState.vm.api}/squads/positions`)
			.then((res) => store.commit(SET_POSITIONS, res.data)).catch((error) => console.warn(error))
	},
	[CREATE_LINEUP]: (store: ActionContext<State, any>, payload: { lineup: CrudLineup }): Promise<Result[]> => {
		return new Promise<Result[]>((resolve, reject) => {
			return axios.post(`${store.rootState.vm.api}/squads/lineups/create`, payload.lineup)
				.then((res) => resolve(res.data)).catch((error) => console.warn(error))
		})
	},
	[FETCH_LINEUP_ON_POST]: (store: ActionContext<State, any>, payload: { postId: number }): Promise<Lineup> => {
		return new Promise<Lineup>((resolve, reject) => {
			return axios.get(`${store.rootState.vm.api}/squads/lineups/post/${payload.postId}`)
				.then((res) => resolve(res.data)).catch((error) => console.warn(error))
		})
	},
	[FETCH_LINEUP]: (store: ActionContext<State, any>, payload: { id: number }): Promise<Lineup> => {
		return new Promise<Lineup>((resolve, reject) => {
			return axios.get(`${store.rootState.vm.api}/squads/lineups/${payload.id}`)
				.then((res) => resolve(res.data)).catch((error) => console.warn(error))
		})
	},
	[CREATE_PLAYER]: (store: ActionContext<State, any>, payload: { player: CrudPlayer }): Promise<Result[]> => {
		return new Promise<Result[]>((resolve, reject) => {
			return axios.post(`${store.rootState.vm.api}/squads/players/create`, payload.player)
				.then((res) => resolve(res.data)).catch((error) => console.warn(error))
		})
	},
	[UPDATE_PLAYER]: (store: ActionContext<State, any>, payload: { player: CrudPlayer }): Promise<Result[]> => {
		return new Promise<Result[]>((resolve, reject) => {
			return axios.post(`${store.rootState.vm.api}/squads/players/update`, payload.player)
				.then((res) => resolve(res.data)).catch((error) => console.warn(error))
		})
	},
}
export default Actions