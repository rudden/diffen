export class Lineup {
    id: number
    formation: Formation
    players: PlayerToLineup[]
    created: string
}

export class PlayerToLineup {
    id: number
    player: Player
    position: Position
}

export class Player {
    id: number
    name: string
    firstName: string
    lastName: string
    fullName: string
    kitNumber: number
    isCaptain: boolean
    isOutOnLoan: boolean
    isHereOnLoan: boolean
    isSold: boolean
    availablePositions: Position[]
    inNumberOFStartingElevens: number
}

export class Formation {
    id: number
    name: string
    componentName: string
}

export class Position {
    id: number
    name: string
}