import { IdAndNickNameUser } from '../common'
import { User } from '../profile'

export class Poll {
    id: number
    name: string
    slug: string
    selections: PollSelection[]
    byUser: IdAndNickNameUser
    created: string
    isOpen: boolean
}

export class PollSelection {
    id: number
    name: string
    votes: IdAndNickNameUser[] = []
    isWinner: boolean
}

export class Chronicle {
    id: number
    title: string
    text: string
    headerFileName: string
    slug: string
    writtenByUser: User
    categories: ChronicleCategory[] = []
    created: string
    updated: string
    published: string
}

export class ChronicleCategory {
    id: number
    name: string
}

export class Region {
    id: number
    name: string
    longitud: number
    latitud: number
    users: IdAndNickNameUser[] = []
}