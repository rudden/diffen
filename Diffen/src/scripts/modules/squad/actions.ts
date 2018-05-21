import axios from 'axios'
import State from './state'
import { Store, ActionTree, ActionContext } from 'vuex'

import { Result, KeyValuePair } from '../../model/common'
import { Lineup, Player, Formation, Title, Game, GameResultGuess, GameResultGuessLeagueItem } from '../../model/squad'
import { Lineup as CrudLineup, Player as CrudPlayer, Game as CrudGame, GameResultGuess as CrudResultGuess } from '../../model/squad/crud'

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
	CREATE_PLAYER,
	FETCH_GAMES,
	SET_GAMES,
	CREATE_GAME,
	UPDATE_GAME,
    FETCH_TITLES,
	FETCH_UPCOMING_GAME,
    GUESS_GAME_RESULT,
    FETCH_FINISHED_GAME_RESULT_GUESSES
} from './types'

axios.defaults.withCredentials = true

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
	[FETCH_GAMES]: (store: ActionContext<State, any>): Promise<void> => {
		return axios.get(`${store.rootState.vm.api}/squads/games`)
			.then((res) => store.commit(SET_GAMES, res.data)).catch((error) => console.warn(error))
	},
	[CREATE_GAME]: (store: ActionContext<State, any>, payload: { game: CrudGame }): Promise<boolean> => {
		return new Promise<boolean>((resolve, reject) => {
			return axios.post(`${store.rootState.vm.api}/squads/games/create`, payload.game)
				.then((res) => resolve(res.data)).catch((error) => console.warn(error))
		})
	},
	[UPDATE_GAME]: (store: ActionContext<State, any>, payload: { game: CrudGame }): Promise<boolean> => {
		return new Promise<boolean>((resolve, reject) => {
			return axios.post(`${store.rootState.vm.api}/squads/games/update`, payload.game)
				.then((res) => resolve(res.data)).catch((error) => console.warn(error))
		})
	},
	[FETCH_TITLES]: (store: ActionContext<State, any>): Promise<Title[]> => {
		return new Promise<Title[]>((resolve, reject) => {
			return axios.get(`${store.rootState.vm.api}/squads/titles`)
				.then((res) => resolve(res.data)).catch((error) => console.warn(error))
		})
	},
	[FETCH_UPCOMING_GAME]: (store: ActionContext<State, any>): Promise<Game> => {
		return new Promise<Game>((resolve, reject) => {
			return axios.get(`${store.rootState.vm.api}/squads/games/upcoming`)
				.then((res) => resolve(res.data)).catch((error) => console.warn(error))
		})
	},
	[FETCH_FINISHED_GAME_RESULT_GUESSES]: (store: ActionContext<State, any>): Promise<GameResultGuessLeagueItem[]> => {
		return new Promise<GameResultGuessLeagueItem[]>((resolve, reject) => {
			return axios.get(`${store.rootState.vm.api}/squads/games/result/finished`)
				.then((res) => resolve(res.data)).catch((error) => console.warn(error))
		})
	},
	[GUESS_GAME_RESULT]: (store: ActionContext<State, any>, payload: { guess: CrudResultGuess }): Promise<boolean> => {
		return new Promise<boolean>((resolve, reject) => {
			return axios.post(`${store.rootState.vm.api}/squads/games/result/guess`, payload.guess)
				.then((res) => {
					if (res) {
						store.rootState.vm.loggedInUser.gameResultGuesses.push({
							game: {
								id: payload.guess.gameId
							},
							numberOfGoalsScoredByDif: payload.guess.numberOfGoalsScoredByDif,
							numberOfGoalsScoredByOpponent: payload.guess.numberOfGoalsScoredByOpponent
						})
					}
					resolve(res.data)
				}).catch((error) => console.warn(error))
		})
	},
}
export default Actions