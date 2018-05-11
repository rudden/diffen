export class PersonalMessage {
    fromUserId: string
    toUserId: string
    message: string
}

export class User {
    nickName: string = ''
    bio: string = ''
    roles: string[] = []
    region: string
    favoritePlayerId: number = 0
    secludeUntil: string = ''
}

export class Invite {
    amount: number = 1
    invitedByUserId: string = ''
}