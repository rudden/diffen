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
    isViceCaptain: boolean
    isOutOnLoan: boolean
    isHereOnLoan: boolean
    isSold: boolean
    availablePositions: Position[]
    inNumberOFStartingElevens: number
    events: PlayerEventOnPlayer[]
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

export class Game {
    id: number
    type: GameType
    playedOn: string
    playerEvents: PlayerEvent[]
}

export class PlayerEvent {
    id: number
    player: Player
    eventType: GameEventType
}

export class PlayerEventOnPlayer {
    gameId: number
    gameType: GameType
    eventType: GameEventType
    date: string
}

export enum GameType {
    Cup,
    League,
    EuropaLeague,
    Training
}

export enum GameEventType {
    Goal,
    Assist,
    YellowCard,
    RedCard
}

export class Title {
    id: number
    type: TitleType
    year: string
    description: string
}

export enum TitleType {
    Cup,
    League
}