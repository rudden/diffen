import { KeyValuePair } from '../../model/common'
import { Lineup, Formation, Player, Position } from '../../model/squad'
import { Lineup as CrudLineup } from '../../model/squad/crud'

export default class State {
    lineups: Lineup[] = []
    formations: Formation[] = []
    positions: Position[] = []
    players: Player[] = []
    newLineup: CrudLineup = new CrudLineup()
    selectedLineup: Lineup = new Lineup()
}