import axios from 'axios'
import moment from 'moment'
import State from './state'
import { Store, ActionTree, ActionContext } from 'vuex'

import { Post, Filter, VoteType, UrlTip, Thread, ThreadType } from '../../model/forum'
import { Post as CrudPost, Vote as CrudVote } from '../../model/forum/crud'
import { Result, Paging } from '../../model/common'
import { Lineup } from '../../model/squad'

import { 
	FETCH_PAGED_POSTS, 
	CREATE_POST, 
	UPDATE_POST, 
	CREATE_VOTE, 
	DELETE_VOTE,
	BOOKMARK_POST,
	SCISSOR_POST,
	FETCH_KVP_USERS,
	FETCH_LINEUP_ON_POST,
	UPDATE_URLTIP_CLICKS,
	SET_PAGED_POSTS, 
	SET_POST_AFTER_VOTE,
	SET_REMOVE_POST_FROM_LIST,
    SET_URLTIP_TOPLIST,
	FETCH_URLTIP_TOPLIST,
	FETCH_CONVERSATION_ON_POST,
    FETCH_POST,
	SET_SELECTED_CONVERSATION,
    FETCH_THREADS,
	SET_THREADS,
	SET_ACTIVE_FIXED_THREAD,
    UPDATE_THREADS_ON_POST
} from './types'

axios.defaults.withCredentials = true

// export everything compliant to the vuex specification for actions
export const Actions: ActionTree<State, any> = {
	[FETCH_POST]: (store: ActionContext<State, any>, payload: { postId: number }): Promise<Post> => {
		return new Promise<Post>((resolve, reject) => {
			return axios.get(`${store.rootState.vm.api}/posts/${payload.postId}`)
				.then((res) => resolve(res.data)).catch((error) => console.warn(error))
		})
	},
	[FETCH_CONVERSATION_ON_POST]: (store: ActionContext<State, any>, payload: { postId: number }): Promise<void> => {
		return axios.get(`${store.rootState.vm.api}/posts/${payload.postId}/conversation`)
			.then((res) => store.commit(SET_SELECTED_CONVERSATION, res.data)).catch((error) => console.warn(error))
	},
	[FETCH_PAGED_POSTS]: (store: ActionContext<State, any>, payload: { pageNumber: number, pageSize: number, filter: Filter, concat: boolean }): Promise<Paging<Post>> => {
		return new Promise<Paging<Post>>((resolve, reject) => {
			return axios.get(`${store.rootState.vm.api}/posts/page/${payload.pageNumber}/${payload.pageSize}?filter=${JSON.stringify(payload.filter)}`)
				.then((res) => {
					store.commit(SET_PAGED_POSTS, { pagedPosts: res.data, concat: payload.concat } )
					resolve(res.data)
				}).catch((error) => console.warn(error))
		})
	},
	[CREATE_POST]: (store: ActionContext<State, any>, payload: { post: CrudPost }): Promise<Result[]> => {
		return new Promise<Result[]>((resolve, reject) => {
			return axios.post(`${store.rootState.vm.api}/posts/create`, payload.post)
				.then((res) => {
					store.rootState.vm.loggedInUser.numberOfPosts++
					resolve(res.data)
				}).catch((error) => console.warn(error))
		})
	},
	[UPDATE_POST]: (store: ActionContext<State, any>, payload: { post: CrudPost }): Promise<Result[]> => {
		return new Promise<Result[]>((resolve, reject) => {
			return axios.post(`${store.rootState.vm.api}/posts/update`, payload.post)
				.then((res) => resolve(res.data)).catch((error) => console.warn(error))
		})
	},
	[CREATE_VOTE]: (store: ActionContext<State, any>, payload: { vote: CrudVote }): Promise<void> => {
		return axios.post(`${store.rootState.vm.api}/posts/vote`, payload.vote)
			.then((res) => {
				if (res.data == 0) return
				switch (payload.vote.type) {
					case VoteType.Up:
						store.rootState.vm.loggedInUser.voteStatistics.upVotes++
						break
					case VoteType.Down:
						store.rootState.vm.loggedInUser.voteStatistics.downVotes++
						break
				}
				store.commit(SET_POST_AFTER_VOTE, { vote: payload.vote, voteId: res.data, nickName: store.rootState.vm.loggedInUser.nickName })
			}).catch((error) => console.warn(error))
	},
	[DELETE_VOTE]: (store: ActionContext<State, any>, payload: { id: number }): Promise<boolean> => {
		return new Promise<boolean>((resolve, reject) => {
			return axios.delete(`${store.rootState.vm.api}/posts/vote/${payload.id}`)
			.then((res) => resolve(res.data)).catch((error) => console.warn(error))
		})
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
	[FETCH_LINEUP_ON_POST]: (store: ActionContext<State, any>, payload: { postId: number }): Promise<Lineup> => {
		return new Promise<Lineup>((resolve, reject) => {
			return axios.get(`${store.rootState.vm.api}/squads/lineups/post/${payload.postId}`)
				.then((res) => resolve(res.data)).catch((error) => console.warn(error))
		})
	},
	[UPDATE_URLTIP_CLICKS]: (store: ActionContext<State, any>, payload: { subject: string, id: number }): Promise<boolean> => {
		return new Promise<boolean>((resolve, reject) => {
			return axios.post(`${store.rootState.vm.api}/posts/url/${payload.subject}/${payload.id}/click`)
				.then((res) => resolve(res.data)).catch((error) => console.warn(error))
		})
	},
	[FETCH_URLTIP_TOPLIST]: (store: ActionContext<State, any>): Promise<void> => {
		return axios.get(`${store.rootState.vm.api}/posts/url/toplist`)
			.then((res) => store.commit(SET_URLTIP_TOPLIST, res.data)).catch((error) => console.warn(error))
	},
	[FETCH_THREADS]: (store: ActionContext<State, any>): Promise<void> => {
		return axios.get(`${store.rootState.vm.api}/posts/threads`)
			.then((res) => {
				store.commit(SET_THREADS, res.data)
				let thread: Thread = res.data.filter((t: Thread) => t.type == ThreadType.Planned && moment(new Date()).isBetween(moment(<string>t.startTime), moment(<string>t.endTime)))[0]
				store.commit(SET_ACTIVE_FIXED_THREAD, thread ? thread : undefined)
			}).catch((error) => console.warn(error))
	},
	[UPDATE_THREADS_ON_POST]: (store: ActionContext<State, any>, payload: { postId: number, threadIds: number[] }): Promise<boolean> => {
		return new Promise<boolean>((resolve, reject) => {
			return axios.post(`${store.rootState.vm.api}/posts/${payload.postId}/threads`, payload.threadIds)
				.then((res) => resolve(res.data)).catch((error) => reject())
		})
	},
}
export default Actions