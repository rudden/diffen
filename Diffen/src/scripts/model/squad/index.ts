import { IdAndNickNameUser } from '../common'

export class Lineup {
    id: number
    formation: Formation
    players: PlayerToLineup[]
    type: LineupType
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
    arenaType: ArenaType
    lineup: Lineup
    opponent: string
    numberOfGoalsScoredByOpponent: number
    playedOn: string
    playerEvents: PlayerEvent[]
}

export class PlayerEvent {
    id: number
    player: EventPlayer
    eventType: GameEventType
    inMinute: number
}

export class EventPlayer {
    id: number
    fullName: string
}

export class PlayerEventOnPlayer {
    gameId: number
    opponent: string
    gameType: GameType
    eventType: GameEventType
    inMinuteOfGame: number
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
    RedCard,
    SubstituteIn,
    SubstituteOut
}

export enum ArenaType {
    Home,
    Away
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

export enum LineupType {
    Real,
    Fiction
}

export class GameResultGuess {
    game: Game
    numberOfGoalsScoredByDif: number
    numberOfGoalsScoredByOpponent: number
}

export class GameResultGuessLeagueItem {
    user: IdAndNickNameUser
    guesses: GameResultGuess[]
}