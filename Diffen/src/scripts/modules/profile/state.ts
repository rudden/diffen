import { KeyValuePair, Paging } from '../../model/common'
import { User } from '../../model/profile'
import { Post } from '../../model/forum'

export default class State {
    user: User = new User()
    createdPosts: Paging<Post> = new Paging<Post>()
    savedPosts: Paging<Post> = new Paging<Post>()
}