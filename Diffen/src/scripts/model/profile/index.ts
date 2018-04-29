import { Lineup as CrudLineup } from '../squad/crud'
import { Player } from '../squad'
import { KeyValuePair } from '../common'

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

    secludedUntil: string
}

export class Filter {
	userId: string
	postsPerPage: number
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
    since: string
}

export class PmUser {
    id: string
    avatar: string
    nickName: string
}

export class Invite {
    email: string
    invitedBy: InvitedBy
    accountIsCreated: boolean
    inviteSent: string
    accountCreated: string
}

export class InvitedBy {
    id: string
    nickName: string
}