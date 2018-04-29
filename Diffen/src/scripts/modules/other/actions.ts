import axios from 'axios'
import State from './state'
import { Store, ActionTree, ActionContext } from 'vuex'

import { Result } from '../../model/common'
import { Poll, PollSelection } from '../../model/other'
import { Poll as CrudPoll, PollVote as CrudVote, Chronicle as CrudChronicle, Region as CrudRegion } from '../../model/other/crud'

import { 
	FETCH_POLL,
	FETCH_POLLS,
	FETCH_ACTIVE_POLLS,
	SET_POLL,
	SET_POLLS,
	CREATE_POLL,
	CREATE_POLL_VOTE,
	CREATE_CHRONICLE,
	FETCH_CHRONICLE,
	FETCH_CHRONICLES,
	SET_CHRONICLE,
	SET_CHRONICLES,
	UPDATE_CHRONICLE,
	SET_VOTES_ON_POLL,
    FETCH_REGIONS,
	SET_REGIONS,
    CREATE_REGION
} from './types'

// export everything compliant to the vuex specification for actions
export const Actions: ActionTree<State, any> = {
	[FETCH_POLL]: (store: ActionContext<State, any>, payload: { slug: string }): Promise<void> => {
		return axios.get(`${store.rootState.vm.api}/polls/${payload.slug}`)
			.then((res) => store.commit(SET_POLL, res.data)).catch((error) => console.warn(error))
	},
	[FETCH_POLLS]: (store: ActionContext<State, any>): Promise<void> => {
		return axios.get(`${store.rootState.vm.api}/polls`)
			.then((res) => store.commit(SET_POLLS, res.data)).catch((error) => console.warn(error))
	},
	[FETCH_ACTIVE_POLLS]: (store: ActionContext<State, any>, payload: { amount: number }): Promise<void> => {
		return axios.get(`${store.rootState.vm.api}/polls/active?amount=${payload.amount}`)
			.then((res) => store.commit(SET_POLLS, res.data)).catch((error) => console.warn(error))
	},
	[CREATE_POLL_VOTE]: (store: ActionContext<State, any>, payload: { pollId: number, vote: CrudVote }): Promise<Result[]> => {
		return new Promise<Result[]>((resolve, reject) => {
			return axios.post(`${store.rootState.vm.api}/polls/${payload.pollId}/vote`, payload.vote) 
				.then((res) => {
					store.commit(SET_VOTES_ON_POLL, { vote: payload.vote, pollId: payload.pollId, byUserNickName: store.rootState.vm.loggedInUser.nickName })
					resolve(res.data)
				}).catch((error) => console.warn(error))
		})
	},
	[CREATE_POLL]: (store: ActionContext<State, any>, payload: { poll: CrudPoll }): Promise<Result[]> => {
		return new Promise<Result[]>((resolve, reject) => {
			return axios.post(`${store.rootState.vm.api}/polls/create`, payload.poll)
				.then((res) => resolve(res.data)).catch((error) => console.warn(error))
		})
	},
	[FETCH_CHRONICLE]: (store: ActionContext<State, any>, payload: { slug: number }): Promise<void> => {
		return axios.get(`${store.rootState.vm.api}/chronicles/${payload.slug}`)
			.then((res) => store.commit(SET_CHRONICLE, res.data)).catch((error) => console.warn(error))
	},
	[FETCH_CHRONICLES]: (store: ActionContext<State, any>, payload: { amount: number }): Promise<void> => {
		return axios.get(`${store.rootState.vm.api}/chronicles?amount=${payload.amount}`)
			.then((res) => store.commit(SET_CHRONICLES, res.data)).catch((error) => console.warn(error))
	},
	[FETCH_REGIONS]: (store: ActionContext<State, any>): Promise<void> => {
		return axios.get(`${store.rootState.vm.api}/regions`)
			.then((res) => store.commit(SET_REGIONS, res.data)).catch((error) => console.warn(error))
	},
	[CREATE_CHRONICLE]: (store: ActionContext<State, any>, payload: { chronicle: CrudChronicle }): Promise<Result[]> => {
		return new Promise<Result[]>((resolve, reject) => {
			return axios.post(`${store.rootState.vm.api}/chronicles/create`, payload.chronicle)
				.then((res) => resolve(res.data)).catch((error) => console.warn(error))
		})
	},
	[UPDATE_CHRONICLE]: (store: ActionContext<State, any>, payload: { chronicle: CrudChronicle }): Promise<Result[]> => {
		return new Promise<Result[]>((resolve, reject) => {
			return axios.post(`${store.rootState.vm.api}/chronicles/update`, payload.chronicle)
				.then((res) => resolve(res.data)).catch((error) => console.warn(error))
		})
	},
	[CREATE_REGION]: (store: ActionContext<State, any>, payload: { region: CrudRegion }): Promise<Result[]> => {
		return new Promise<Result[]>((resolve, reject) => {
			return axios.post(`${store.rootState.vm.api}/regions/create`, payload.region)
				.then((res) => resolve(res.data)).catch((error) => console.warn(error))
		})
	},
}
export default Actions