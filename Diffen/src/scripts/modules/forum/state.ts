import { Paging, KeyValuePair } from '../../model/common'
import { Post, Filter } from '../../model/forum'

export default class State {
    pagedPosts: Paging<Post> = new Paging<Post>()
    isLoadingPosts: boolean = true
    filter: Filter = new Filter()
    users: KeyValuePair[] = []
}