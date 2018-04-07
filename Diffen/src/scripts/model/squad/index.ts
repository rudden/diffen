export class Lineup {
    id: number
    players: PlayerToLineup[]
    created: string
}

export class PlayerToLineup {
    id: number
    player: Player
    position: string
}

export class Player {
    id: number
    name: string
    firstName: string
    lastName: string
    kitNumber: number
    isCaptain: boolean
    isOutOnLoan: boolean
    isHereOnLoan: boolean
    isSold: boolean
}