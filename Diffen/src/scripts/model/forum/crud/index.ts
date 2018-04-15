import { Lineup } from '../../squad/crud'
import { VoteType } from '..'

export class Post {
    id: number
    message: string
    createdByUserId: string
    parentPostId?: number
    urlTipHref?: string
    lineup?: Lineup
}

export class Vote {
    type: VoteType
    postId: number
    createdByUserId: string
}