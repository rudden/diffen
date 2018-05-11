export class Poll {
    name: string = ''
    selections: string[] = []
    createdByUserId: string
}

export class PollVote {
    pollSelectionId: number
    votedByUserId: string
}

export class Chronicle {
    id: number
    title: string = ''
    text: string = ''
    writtenByUserId: string
    categoryIds: number[]
    newCategoryNames?: string[]
    published: string
}

export class Region {
    name: string = ''
    longitud: number = 0
    latitud: number = 0
}

export class ChronicleCategory {
    name: string
}