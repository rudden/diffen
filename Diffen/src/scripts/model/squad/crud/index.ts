import { GameType, GameEventType } from ".."

export class Lineup {
    id?: number
    formationId: number
    players: PlayerToLineup[] = []
    createdByUserId: string
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
    isHereOnLoan: boolean
    isOutOnLoan: boolean
    availablePositionsIds: number[] = []
}

export class Game {
    type: GameType
    playedDate: string = ''
    playerEvents: PlayerEvent[] = []
}

export class PlayerEvent {
    playerId: number
    type: GameEventType
}