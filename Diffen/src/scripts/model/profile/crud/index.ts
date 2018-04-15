export class PersonalMessage {
    fromUserId: string
    toUserId: string
    message: string
}

export class User {
    nickName: string = ''
    bio: string = ''
    roles: string[] = []
    favoritePlayerId: number = 0
}

export class Invite {
    email: string = ''
    invitedByUserId: string = ''
}