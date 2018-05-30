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
    player: PlayerToLineupPlayer
    position: Position
}

export class PlayerToLineupPlayer {
    id: number
    fullName: string
    shortName: string
    attributes: PlayerAttributes
    availablePositions: Position[]
}

export class PlayerAttributes {
    isCaptain: boolean
    isViceCaptain: boolean
    isOutOnLoan: boolean
    isHereOnLoan: boolean
    isSold: boolean
}

export class Player {
    id: number
    name: string
    firstName: string
    lastName: string
    fullName: string
    kitNumber: number
    attributes: PlayerAttributes
    birthDay: string
    heightInCentimeters: number
    weight: number
    preferredFoot: PreferredFoot
    about: string
    contractUntil: string
    imageUrl: string
    availablePositions: Position[]
    statistics: PlayerStatistics
}

export class PlayerStatistics {
    events: PlayerEventOnPlayer[]
    gamesWithoutEvents: Game[]
    distinctGamesWithEvents: Game[]
}

export class PlayerTableData {
	numberOfGames: number = 0
	numberOfGamesFromStart: number = 0
	numberOfGamesSubstituteOut: number = 0
	numberOfGamesSubstituteIn: number = 0
	numberOfMinutesPlayed: number = 0
	numberOfGoals: number = 0
	numberOfAssists: number = 0
	numberOfYellowCards: number = 0
	numberOfRedCards: number = 0
    numberOfPoints: number = 0
    numberOfMinutesPerPoint: number = 0
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
    numberOfAddonMinutes: number
    tablePlacementAfterGame: number
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
    Training,
    All
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
    Away,
    NeutralGround
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

export enum PreferredFoot {
    None,
    Left,
    Right
}

export class Season {
    id: number
    name: string
    isActive: boolean
    games: Game[]
}