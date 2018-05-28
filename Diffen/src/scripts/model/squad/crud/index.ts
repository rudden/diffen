import { GameType, GameEventType, LineupType, ArenaType, PreferredFoot, PlayerAttributes } from ".."

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
    attributes: PlayerAttributes = new PlayerAttributes()
    availablePositionsIds: number[] = []
    birthDay?: Date = new Date()
    heightInCentimeters: number
    weight: number
    preferredFoot: PreferredFoot
    about: string = ''
    contractUntil?: Date = new Date()
    imageUrl: string = ''
}

export class Game {
    id?: number
    type: GameType
    arenaType: ArenaType
    lineup?: Lineup
    opponent: string
    numberOfGoalsScoredByOpponent: number
    numberOfAddonMinutes: number
    playedDate: string
    events: PlayerEvent[] = []
}

export class PlayerEvent {
    id: number = 0
    playerId: number = 0
    type: GameEventType = GameEventType.Goal
    inMinute: number = 0

    guid?: string = ''

    /**
     *
     */
    constructor(guid?: string) {
        this.guid = guid
    }
}

export class GameResultGuess {
    gameId: number
    numberOfGoalsScoredByDif: number
    numberOfGoalsScoredByOpponent: number
    guessedByUserId: string
}