import { KeyValuePair } from '../../model/common'
import { User } from '../../model/profile'

export default class State {
    user: User = new User()
    users: KeyValuePair[] = []
}