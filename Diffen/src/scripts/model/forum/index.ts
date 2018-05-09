import { KeyValuePair } from '../common'

export class Post {
    id: number
    message: string

    user: User
    urlTipHref: string
    votes: Vote[]
    parentPost: ParentPost
    lineupId?: number

    since: string
    updated: string

    isScissored: boolean
    loggedInUserCanVote: boolean

    inEdit: boolean
    inReply: boolean
    inScissor: boolean
    disabled: boolean
}

export class User {
    id: string = ''
    avatar: string = ''
    nickName: string = ''
    isAdmin: boolean = false
    secludedUntil: string = ''
}

export class ParentPost {
    id: number
    message: string
    user: User
    since: string
}

export class Vote {
    type: VoteType
    byNickName: string = ''
}

export enum VoteType {
    Up,
    Down
}

export enum StartingEleven {
	All, With, Without
}

export class Filter {
    fromDate?: Date
    toDate?: Date
    messageWildCard?: string
    startingEleven?: StartingEleven = StartingEleven.All
	includedUsers?: KeyValuePair[]
	excludedUsers?: KeyValuePair[]
}

export class UrlTip {
    id: number
    href: string
    clicks: number
    postId?: number
}

export class Conversation {
    post: Post
    children: Conversation[]
    all?: Post[]
}