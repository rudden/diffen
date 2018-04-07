import { Player } from '../squad'
import { KeyValuePair } from '../common'

export class User {
	id: string
	bio: string
	nickName: string
	avatar: string

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
    upvotes: number
    downvotes: number
}