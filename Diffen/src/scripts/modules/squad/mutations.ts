import State from './state'
import { MutationTree } from 'vuex'

import { User, PersonalMessage } from '../../model/profile'
import { KeyValuePair } from '../../model/common'
import { Formation, Lineup, Player, Position, Game } from '../../model/squad'
import { PlayerToLineup, Lineup as CrudLineup } from '../../model/squad/crud'

import { 
    SET_FORMATIONS,
    SET_LINEUPS,
    SET_PLAYERS,
    SET_POSITIONS,
    SET_SELECTED_LINEUP,
    SET_NEW_LINEUP,
    SET_PLAYER_TO_LINEUP,
    SET_GAMES
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
    [SET_GAMES]: (state: State, games: Game[]) => { state.games = games }
}

export default Mutations