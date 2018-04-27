import axios from 'axios'
import State from './state'
import { Store, ActionTree, ActionContext } from 'vuex'

import { Result } from '../../model/common'
import { Poll, PollSelection } from '../../model/other'
import { Poll as CrudPoll, PollVote as CrudVote, Chronicle as CrudChronicle } from '../../model/other/crud'

import { 
	FETCH_POLLS,
	FETCH_ACTIVE_POLLS,
	SET_POLLS,
	CREATE_POLL,
	CREATE_POLL_VOTE,
	CREATE_CHRONICLE,
	FETCH_CHRONICLE,
	FETCH_CHRONICLES,
	SET_CHRONICLE,
	SET_CHRONICLES,
	UPDATE_CHRONICLE
} from './types'

// export everything compliant to the vuex specification for actions
export const Actions: ActionTree<State, any> = {
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
					store.state.polls.filter((p: Poll) => p.id == payload.pollId)[0]
						.selections.filter((s: PollSelection) => s.id == payload.vote.pollSelectionId)[0]
							.votes.push({
								id: payload.vote.votedByUserId,
								nickName: store.rootState.vm.loggedInUser.nickName
							})
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
}
export default Actions