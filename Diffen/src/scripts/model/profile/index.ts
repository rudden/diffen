import { Lineup as CrudLineup } from '../squad/crud'
import { Player, GameResultGuess } from '../squad'
import { KeyValuePair, IdAndNickNameUser } from '../common'

export class User {
	id: string
	bio: string
	nickName: string
	avatar: string
    region: string

    karma: number
    numberOfPosts: number
    
    voteStatistics: VoteStatistics = new VoteStatistics()
    filter: Filter = new Filter()
    favoritePlayer: Player = new Player()
    savedPostsIds: number[]
    inRoles: string[]
    numberOfUnReadPersonalMessages: number
    gameResultGuesses: GameResultGuess[]

    joined: string
    secludedUntil: string
}

export class Filter {
	userId: string
    postsPerPage: number
    hideLeftMenu: boolean
    hideRightMenu: boolean
	excludedUsers: KeyValuePair[] = []
}

export class VoteStatistics {
    upVotes: number
    downVotes: number
}

export class PersonalMessage {
    id: number
    from: PmUser
    to: PmUser
    message: string
    isReadByToUser: boolean
    since: string
}

export class Conversation {
    user: IdAndNickNameUser
    numberOfUnReadMessages: number
}

export class PmUser {
    id: string
    avatar: string
    nickName: string
}

export class Invite {
    uniqueCode: string
    invitedBy: InvitedBy
    inviteUsedBy?: InvitedBy
    accountIsCreated: boolean
    inviteSent: string
    accountCreated: string
}

export class InvitedBy {
    id: string
    nickName: string
}