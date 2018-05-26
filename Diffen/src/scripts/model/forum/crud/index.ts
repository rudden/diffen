import { Lineup } from '../../squad/crud'
import { VoteType } from '..'

export class Post {
    id: number
    message: string
    createdByUserId: string
    parentPostId?: number
    urlTipHref?: string
    lineup?: Lineup
    lineupId: number = 0
    threadIds: number[] = []
    newThreadNames?: string[] = []
}

export class Vote {
    type: VoteType
    postId: number
    createdByUserId: string
}