import axios from 'axios'
import State from './state'
import { Store, ActionTree, ActionContext } from 'vuex'

import { Post, Filter } from '../../model/forum'
import { Post as CrudPost, Vote as CrudVote } from '../../model/forum/crud'
import { Result } from '../../model/common'

import { 
	FETCH_PAGED_POSTS, 
	CREATE_POST, 
	UPDATE_POST, 
	CREATE_VOTE, 
	BOOKMARK_POST,
	SCISSOR_POST,
	FETCH_KVP_USERS,
	SET_PAGED_POSTS, 
	SET_POST_AFTER_VOTE,
	SET_REMOVE_POST_FROM_LIST,
    SET_KVP_USERS
} from './types'

// export everything compliant to the vuex specification for actions
export const Actions: ActionTree<State, any> = {
	[FETCH_PAGED_POSTS]: (store: ActionContext<State, any>, payload: { pageNumber: number, pageSize: number, filter: Filter }): Promise<void> => {
		return axios.get(`${store.rootState.vm.api}/posts/page/${payload.pageNumber}/${payload.pageSize}?filter=${JSON.stringify(payload.filter)}`)
			.then((res) => store.commit(SET_PAGED_POSTS, res.data)).catch((error) => console.warn(error))
	},
	[CREATE_POST]: (store: ActionContext<State, any>, payload: { post: CrudPost }): Promise<Result[]> => {
		return new Promise<Result[]>((resolve, reject) => {
			return axios.post(`${store.rootState.vm.api}/posts/${store.rootState.vm.loggedInUser.id}/create`, payload.post)
				.then((res) => {
					store.rootState.vm.loggedInUser.numberOfPosts++
					resolve(res.data)
				}).catch((error) => console.warn(error))
		})
	},
	[UPDATE_POST]: (store: ActionContext<State, any>, payload: { post: CrudPost }): Promise<Result[]> => {
		return new Promise<Result[]>((resolve, reject) => {
			return axios.post(`${store.rootState.vm.api}/posts/${store.rootState.vm.loggedInUser.id}/update`, payload.post)
				.then((res) => resolve(res.data)).catch((error) => console.warn(error))
		})
	},
	[CREATE_VOTE]: (store: ActionContext<State, any>, payload: { vote: CrudVote }): Promise<void> => {
		return axios.post(`${store.rootState.vm.api}/posts/vote`, payload.vote)
			.then((res) => {
				if (!res) return
				store.commit(SET_POST_AFTER_VOTE, { vote: payload.vote, nickName: store.rootState.vm.loggedInUser.nickName })
			}).catch((error) => console.warn(error))
	},
	[BOOKMARK_POST]: (store: ActionContext<State, any>, payload: { postId: number }): Promise<void> => {
		return axios.post(`${store.rootState.vm.api}/posts/${payload.postId}/bookmark?userId=${store.rootState.vm.loggedInUser.id}`)
			.then((res) => {
				if (!res) return
				store.rootState.vm.loggedInUser.savedPostsIds.push(payload.postId)
			}).catch((error) => console.warn(error))
	},
	[SCISSOR_POST]: (store: ActionContext<State, any>, payload: { postId: number }): Promise<void> => {
		return axios.post(`${store.rootState.vm.api}/posts/${payload.postId}/scissor`)
			.then((res) => {
				if (!res) return
				store.commit(SET_REMOVE_POST_FROM_LIST, payload.postId)
			}).catch((error) => console.warn(error))
	},
	[FETCH_KVP_USERS]: (store: ActionContext<State, any>): Promise<void> => {
		return axios.get(`${store.rootState.vm.api}/users`)
			.then((res) => store.commit(SET_KVP_USERS, res.data)).catch((error) => console.warn(error))
	},
}
export default Actions