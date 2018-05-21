import { GameType, GameEventType, LineupType, ArenaType } from ".."

export class Lineup {
    id?: number
    formationId: number
    players: PlayerToLineup[] = []
    createdByUserId: string
    type: LineupType
    created?: string
}

export class PlayerToLineup {
    playerId: number
    positionId: number
}

export class Player {
    id: number
    firstName: string = ''
    lastName: string = ''
    kitNumber: number = 0
    isSold: boolean
    isCaptain: boolean
    isViceCaptain: boolean
    isHereOnLoan: boolean
    isOutOnLoan: boolean
    availablePositionsIds: number[] = []
}

export class Game {
    id?: number
    type: GameType
    arenaType: ArenaType
    lineup?: Lineup
    opponent: string
    numberOfGoalsScoredByOpponent: number
    playedDate?: Date
    events: PlayerEvent[] = []
}

export class PlayerEvent {
    playerId: number = 0
    type: GameEventType = GameEventType.Goal
    inMinute: number = 0
}

export class GameResultGuess {
    gameId: number
    numberOfGoalsScoredByDif: number
    numberOfGoalsScoredByOpponent: number
    guessedByUserId: string
}