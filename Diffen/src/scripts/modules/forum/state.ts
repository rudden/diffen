import { Paging, KeyValuePair } from '../../model/common'
import { Post, Filter, UrlTip, Conversation, Thread } from '../../model/forum'

export default class State {
    pagedPosts: Paging<Post> = new Paging<Post>()
    selectedConversation: Conversation = new Conversation()
    isLoadingPosts: boolean = true
    filter: Filter = new Filter()
    urlTipTopList: UrlTip[] = []
    showLeftSideBar: boolean = true
    showRightSideBar: boolean = true
    shouldReloadPostStream: boolean = false
    threads: Thread[] = []
    activeFixedThread: Thread = new Thread()
}