import { KeyValuePair } from '../../model/common'
import { Lineup, Formation, Player, Position, Game } from '../../model/squad'
import { Lineup as CrudLineup, Game as CrudGame } from '../../model/squad/crud'

export default class State {
    lineups: Lineup[] = []
    formations: Formation[] = []
    positions: Position[] = []
    players: Player[] = []
    games: Game[] = []
    newLineup: CrudLineup = new CrudLineup()
    selectedLineup: Lineup = new Lineup()
    crudGame: CrudGame = new CrudGame()
}