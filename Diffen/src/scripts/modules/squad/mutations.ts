import State from './state'
import { MutationTree } from 'vuex'

import { User, PersonalMessage } from '../../model/profile'
import { KeyValuePair } from '../../model/common'
import { Formation, Lineup, Player, Position, Game } from '../../model/squad'
import { PlayerToLineup, Lineup as CrudLineup, PlayerEvent, Game as CrudGame } from '../../model/squad/crud'

import { 
    SET_FORMATIONS,
    SET_LINEUPS,
    SET_PLAYERS,
    SET_POSITIONS,
    SET_SELECTED_LINEUP,
    SET_NEW_LINEUP,
    SET_PLAYER_TO_LINEUP,
    SET_GAMES,
    SET_GAME_EVENT,
    CHANGE_GAME_EVENT,
    DELETE_GAME_EVENT,
    SET_CRUD_GAME,
    SET_GAME_EVENTS
} from './types'

export const Mutations: MutationTree<State> = {
    [SET_FORMATIONS]: (state: State, formations: Formation[]) => { state.formations = formations },
    [SET_LINEUPS]: (state: State, lineups: Lineup[]) => { state.lineups = lineups },
    [SET_PLAYERS]: (state: State, players: Player[]) => { state.players = players },
    [SET_POSITIONS]: (state: State, positions: Position[]) => { state.positions = positions },
    [SET_SELECTED_LINEUP]: (state: State, lineup: Lineup) => { state.selectedLineup = lineup },
    [SET_NEW_LINEUP]: (state: State, payload: { formationId: number }) => { 
        state.newLineup = new CrudLineup()
        state.newLineup.formationId = payload.formationId
    },
    [SET_PLAYER_TO_LINEUP]: (state: State, payload: { playerId: number, positionId: number }) => {
		if (payload.playerId == 0) {
			let index: number = 0
			for (let i = 0; i < state.newLineup.players.length; i++) {
				if (state.newLineup.players[i].positionId !== payload.positionId)
					continue
				
				index = state.newLineup.players.indexOf(state.newLineup.players[i])
				break
			}
			if (index > -1)
				state.newLineup.players.splice(index, 1)
		} else {
			state.newLineup.players.push({
                playerId: payload.playerId,
                positionId: payload.positionId
            })
		}
    },
    [SET_GAMES]: (state: State, games: Game[]) => { state.games = games },
    [SET_CRUD_GAME]: (state: State, game: CrudGame) => { state.crudGame = game },
    [SET_GAME_EVENT]: (state: State, event: PlayerEvent) => {
        state.crudGame.events.unshift(event)
    },
    [SET_GAME_EVENTS]: (state: State, events: PlayerEvent[]) => {
        state.crudGame.events = events
    },
    [CHANGE_GAME_EVENT]: (state: State, event: PlayerEvent) => {
        let index = state.crudGame.events.indexOf(event)
        if (index !== -1) {
            state.crudGame.events[index] = event
        }
    },
    [DELETE_GAME_EVENT]: (state: State, event: PlayerEvent) => {
        let index = state.crudGame.events.indexOf(event)
        if (index !== -1) {
            state.crudGame.events.splice(index, 1)
        }
    },
}

export default Mutations