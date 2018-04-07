import { Lineup } from '../../squad/crud'
import { VoteType } from '..'

export class Post {
    id: number
    message: string
    createdByUserId: string
    parentPostId?: number
    urlTip: UrlTip = new UrlTip()
    lineup: Lineup
}

export class UrlTip {
    href: string
}

export class Vote {
    type: VoteType
    postId: number
    createdByUserId: string
}