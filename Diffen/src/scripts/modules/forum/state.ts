import { Paging, KeyValuePair } from '../../model/common'
import { Post, Filter, UrlTip } from '../../model/forum'

export default class State {
    pagedPosts: Paging<Post> = new Paging<Post>()
    isLoadingPosts: boolean = true
    filter: Filter = new Filter()
    urlTipTopList: UrlTip[] = []
    showLeftSideBar: boolean = true
    showRightSideBar: boolean = true
    shouldReloadPostStream: boolean = false
}