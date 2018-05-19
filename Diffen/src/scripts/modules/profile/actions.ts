import axios from 'axios'
import State from './state'
import { Store, ActionTree, ActionContext } from 'vuex'

import { Result, KeyValuePair, Paging } from '../../model/common'
import { User, PersonalMessage, Filter, Invite } from '../../model/profile'
import { User as CrudUser } from '../../model/profile/crud'
import { PersonalMessage as CrudPm, Invite as CrudInvite } from '../../model/profile/crud'
import { Post } from '../../model/forum'

import { 
	FETCH_USER,
	FETCH_KVP_USERS,
	FETCH_PERSONAL_MESSAGES,
	FETCH_POSTS,
	FETCH_SAVED_POSTS,
	FETCH_ROLES,
	CREATE_PM,
	SET_USER,
	SET_CREATED_POSTS,
	SET_SAVED_POSTS,
	SECLUDE_USER,
	UPDATE_USER,
    CHANGE_FILTER,
	FETCH_INVITES,
	CREATE_INVITE,
	FETCH_CONVERSATION_KVP_USERS,
	UNBOOKMARK_POST
} from './types'

axios.defaults.withCredentials = true

// export everything compliant to the vuex specification for actions
export const Actions: ActionTree<State, any> = {
	[FETCH_USER]: (store: ActionContext<State, any>, payload: { id: string }): Promise<void> => {
		return axios.get(`${store.rootState.vm.api}/users/${payload.id}`)
			.then((res) => store.commit(SET_USER, res.data)).catch((error) => console.warn(error))
	},
	[FETCH_KVP_USERS]: (store: ActionContext<State, any>): Promise<KeyValuePair[]> => {
		return new Promise<KeyValuePair[]>((resolve, reject) => {
			return axios.get(`${store.rootState.vm.api}/users`)
				.then((res) => resolve(res.data)).catch((error) => console.warn(error))
		})
	},
	[FETCH_CONVERSATION_KVP_USERS]: (store: ActionContext<State, any>, payload: { userId: string }): Promise<KeyValuePair[]> => {
		return new Promise<KeyValuePair[]>((resolve, reject) => {
			return axios.get(`${store.rootState.vm.api}/users/${payload.userId}/pm/users`)
				.then((res) => resolve(res.data)).catch((error) => console.warn(error))
		})
	},
	[FETCH_PERSONAL_MESSAGES]: (store: ActionContext<State, any>, payload: { to?: string }): Promise<PersonalMessage[]> => {
		return new Promise<PersonalMessage[]>((resolve, reject) => {
			return axios.get(`${store.rootState.vm.api}/users/${store.rootState.vm.loggedInUser.id}/pm${payload.to ? `?to=${payload.to}` : ''}`)
				.then((res) => resolve(res.data)).catch((error) => reject())
		})
	},
	[CREATE_PM]: (store: ActionContext<State, any>, payload: { pm: CrudPm }): Promise<Result[]> => {
		return new Promise<Result[]>((resolve, reject) => {
			return axios.post(`${store.rootState.vm.api}/users/pm/create`, payload.pm)
				.then((res) => resolve(res.data)).catch((error) => console.warn(error))
		})
	},
	[UPDATE_USER]: (store: ActionContext<State, any>, payload: { userId: string, user: CrudUser }): Promise<Result[]> => {
		return new Promise<Result[]>((resolve, reject) => {
			return axios.post(`${store.rootState.vm.api}/users/${payload.userId}/update`, payload.user)
				.then((res) => resolve(res.data)).catch((error) => console.warn(error))
		})
	},
	[CHANGE_FILTER]: (store: ActionContext<State, any>, payload: { filter: Filter }): Promise<Result[]> => {
		return new Promise<Result[]>((resolve, reject) => {
			return axios.post(`${store.rootState.vm.api}/users/filter`, payload.filter)
				.then((res) => resolve(res.data)).catch((error) => console.warn(error))
		})
	},
	[FETCH_POSTS]: (store: ActionContext<State, any>, payload: { userId: string, pageNumber: number }): Promise<void> => {
		return axios.get(`${store.rootState.vm.api}/users/${payload.userId}/posts/${payload.pageNumber}`)
			.then((res) => store.commit(SET_CREATED_POSTS, res.data)).catch((error) => console.warn(error))
	},
	[FETCH_SAVED_POSTS]: (store: ActionContext<State, any>, payload: { userId: string, pageNumber: number }): Promise<void> => {
		return axios.get(`${store.rootState.vm.api}/users/${payload.userId}/posts/saved/${payload.pageNumber}`)
			.then((res) => store.commit(SET_SAVED_POSTS, res.data)).catch((error) => console.warn(error))
	},
	[FETCH_ROLES]: (store: ActionContext<State, any>, payload: { userId: string, pageNumber: number }): Promise<string[]> => {
		return new Promise<string[]>((resolve, reject) => {
			return axios.get(`${store.rootState.vm.api}/users/roles`)
				.then((res) => resolve(res.data)).catch((error) => console.warn(error))
		})
	},
	[FETCH_INVITES]: (store: ActionContext<State, any>): Promise<Invite> => {
		return new Promise<Invite>((resolve, reject) => {
			return axios.get(`${store.rootState.vm.api}/users/invites`)
				.then((res) => resolve(res.data)).catch((error) => console.warn(error))
		})
	},
	[CREATE_INVITE]: (store: ActionContext<State, any>, payload: { invite: CrudInvite }): Promise<string[]> => {
		return new Promise<string[]>((resolve, reject) => {
			return axios.post(`${store.rootState.vm.api}/users/invites/create`, payload.invite)
				.then((res) => resolve(res.data)).catch((error) => console.warn(error))
		})
	},
	[SECLUDE_USER]: (store: ActionContext<State, any>, payload: { userId: string, to: string }): Promise<Result[]> => {
		return new Promise<Result[]>((resolve, reject) => {
			return axios.post(`${store.rootState.vm.api}/users/${payload.userId}/seclude?to=${payload.to}`)
				.then((res) => resolve(res.data)).catch((error) => console.warn(error))
		})
	},
	[UNBOOKMARK_POST]: (store: ActionContext<State, any>, payload: { postId: number }): Promise<void> => {
		return axios.delete(`${store.rootState.vm.api}/posts/${payload.postId}/bookmark?userId=${store.rootState.vm.loggedInUser.id}`)
			.then((res) => {
				if (!res) return
				var index = store.state.savedPosts.data.indexOf(store.state.savedPosts.data.filter((post: Post) => post.id == payload.postId)[0])
				if (index !== -1) {
					store.state.savedPosts.data.splice(index, 1)
				}
			}).catch((error) => console.warn(error))
	},
}
export default Actions