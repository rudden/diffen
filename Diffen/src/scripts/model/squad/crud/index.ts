export class Lineup {
    id: number
    formationId: number
    players: PlayerToLineup[]
    createdByUserId: string
    created: string
}

export class PlayerToLineup {
    playerId: number
    positionId: number
}