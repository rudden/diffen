<template>
    <div>
        <div class="input-group">
            <textarea rows="5" class="form-control" placeholder="Ditt inlägg.." v-model="newPost.message" ref="textarea"></textarea>
        </div>
        <div class="input-group">
            <input type="text" class="form-control br br__br-4" placeholder="Länktips" v-model="newPost.urlTipHref">
            <div class="input-group-btn" v-if="!activeThread">
                <button class="btn btn-secondary ml-2" @click="showThreadSelection = !showThreadSelection" v-tooltip="threadsTooltip">{{ threadsBtnText }}</button>
            </div>
            <div class="input-group-btn">
                <img src="/field-green.png" class="rounded ml-2" style="height: 33px; cursor: pointer" @click="fetchLineups" v-tooltip="'Lägg till startelva'" />
            </div>
            <div class="input-group-btn">
                <button class="btn btn-primary align-self-stretch ml-2" v-on:click="submit" :disabled="!canSubmit">{{ btnText }}</button>
            </div>
        </div>
        <template v-if="showThreadSelection && !activeThread">
            <template v-if="postIsInPlannedThread">
                <div class="row mt-3">
                    <div class="col">
                        <div class="alert alert-warning mb-0">Detta inlägg är del av en planerad tråd. Du kan därför inte justera taggarna.</div>
                    </div>
                </div>
            </template>
            <template v-else>
                <div class="card mt-3">
                    <div class="card-body">
                        <div class="row">
                            <div class="col">
                                <input type="text" v-model="newThreadName" class="form-control form-control-sm" placeholder="Skapa en ny tråd" v-on:keyup.enter="addToSelections" :disabled="selectedThreads.length == 2" />
                            </div>
                        </div>
                        <div class="row mt-3">
                            <div class="col">
                                <div class="form-check form-check-inline" v-for="thread in allThreads" :key="thread.name">
                                    <input class="form-check-input" type="checkbox" :id="thread.name" :value="thread" v-model="selectedThreads" :disabled="disabledThreadSelection(thread.name)">
                                    <label class="form-check-label" :for="thread.name">{{ thread.name }}</label>
                                </div>
                            </div>
                        </div>
                        <div class="row mt-3" v-if="maxNumberOfThreadsSelected">
                            <div class="col">
                                <div class="alert alert-info mb-0">
                                    <span class="icon icon-info mr-2"></span>Du kan inte tagga ett inlägg med mer än 2 trådar
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </template>
        </template>
        <template v-if="showLineupSelection">
            <div class="col mt-3 pl-0 pr-0" v-if="lineups.length > 0 || noLineupsFound">
                <select class="form-control" v-model="newPost.lineupId" :disabled="!lineups.length > 0" @change="changeLineup">
                    <option value="0">{{ lineups.length > 0 ? 'Välj en startelva' : 'Hittade inga startelvor' }}</option>
                    <option v-for="lineup in lineups" :value="lineup.id" :key="lineup.id">{{ lineup.formation.name }}, skapad {{ lineup.created }}</option>
                </select>
            </div>
            <template v-if="newPost.lineupId > 0">
                <div class="mt-3">
                    <formation-component :formation="selectedLineup.formation" :players="selectedLineup.players" />
                </div>
            </template>
        </template>
        <results :items="results" class="pt-3" />
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component, Watch } from 'vue-property-decorator'
import { Getter, Mutation, Action, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('forum', Getter)
const ModuleAction = namespace('forum', Action)
const ModuleMutation = namespace('forum', Mutation)

const SquadModuleGetter = namespace('squad', Getter)
const SquadModuleAction = namespace('squad', Action)
const SquadModuleMutation = namespace('squad', Mutation)

import { GET_ACTIVE_FIXED_THREAD, GET_PAGED_POSTS, GET_FILTER, GET_THREADS, CREATE_POST, UPDATE_POST, FETCH_PAGED_POSTS, FETCH_THREADS, SET_IS_LOADING_POSTS, SET_SHOULD_RELOAD_POST_STREAM } from '../../modules/forum/types'
import { GET_LINEUPS, GET_SELECTED_LINEUP, FETCH_LINEUPS_ON_USER, SET_SELECTED_LINEUP } from '../../modules/squad/types'

import { Post, Filter, Thread, ThreadType } from '../../model/forum'
import { Post as CrudPost } from '../../model/forum/crud'
import { PageViewModel, Result, ResultType, Paging } from '../../model/common'
import { Lineup, PlayerToLineup } from '../../model/squad'
import { Lineup as CrudLineup } from '../../model/squad/crud'

import Modal from '../modal.vue'
import Results from '../results.vue'
import Lineups from '../lineups/lineups.vue'
import FormationComponent from '../lineups/formation.vue'

import moment from 'moment'

@Component({
    props: {
        post: Object,
        parentId: {
            type: Number,
            default: 0
        },
        modalName: {
            type: String,
            default: undefined
        }
    },
    components: {
        Results, FormationComponent, Modal, Lineups
    }
})
export default class NewPost extends Vue {
  	@State(state => state.vm) vm: PageViewModel
    @ModuleGetter(GET_ACTIVE_FIXED_THREAD) activeThread: Thread
    @ModuleGetter(GET_FILTER) filter: Filter
    @ModuleGetter(GET_PAGED_POSTS) pagedPosts: Paging<Post>
    @ModuleGetter(GET_THREADS) threads: Thread[]
    @ModuleAction(CREATE_POST) create: (payload: { post: CrudPost }) => Promise<Result[]>
    @ModuleAction(UPDATE_POST) update: (payload: { post: CrudPost }) => Promise<Result[]>
	@ModuleAction(FETCH_PAGED_POSTS) loadPaged: (payload: { pageNumber: number, pageSize: number, filter: Filter }) => Promise<void>
	@ModuleAction(FETCH_THREADS) loadThreads: () => Promise<void>
	@ModuleMutation(SET_IS_LOADING_POSTS) setIsLoadingPosts: (payload: { value: boolean }) => void
	@ModuleMutation(SET_SHOULD_RELOAD_POST_STREAM) setShouldReloadPostStream: (payload: { value: boolean }) => void

	@SquadModuleGetter(GET_LINEUPS) lineups: Lineup[]
    @SquadModuleGetter(GET_SELECTED_LINEUP) selectedLineup: Lineup
	@SquadModuleAction(FETCH_LINEUPS_ON_USER) loadLineups: (payload: { userId: string }) => Promise<void>
	@SquadModuleMutation(SET_SELECTED_LINEUP) setSelectedLineup: (lineup: Lineup) => void
    
    post: Post
    parentId: number
    modalName: string

    loading: boolean = true
    noLineupsFound: boolean = false
    newPost: CrudPost = new CrudPost()
    results: Result[] = []

    newThreadName: string = ''
    newThreads: Thread[] = []
    selectedThreads: Thread[] = []

    showLineupSelection: boolean = false
    showThreadSelection: boolean = false

    $modal = (this as any).VModal

    modalAttributes: any = {
        lineups: {
            attributes: {
                name: 'post-lu',
                clickToClose: false
            },
            header: 'Lägg till startelva',
            button: {
                image: {
                    classes: 'rounded ml-2',
                    src: '/field-bw.png',
                    style: 'height: 33px'
                },
                text: 'Lägg till startelva'
            }
        }
    }

    created() {
        this.$nextTick(() => {
            (this as any).$refs.textarea.focus()
        })

        this.loadThreads()

        if (this.post) {
            this.newPost.id = this.post.id
            this.newPost.message = this.post.message
            this.newPost.urlTipHref = this.post.urlTipHref
            if (this.post.lineupId) {
                this.loadLineups({ userId: this.vm.loggedInUser.id })
                    .then(() => {
                        this.noLineupsFound = this.lineups.length == 0
                        this.newPost.lineupId = this.post.lineupId ? this.post.lineupId : 0
                        this.changeLineup()
                    })
            } else {
                this.newPost.lineupId = 0
            }
            if (this.post.inThreads) {
                this.selectedThreads = this.threads.filter((t: Thread) => this.post.inThreads.map((t2: Thread) => t2.id).includes(t.id))
            }
        }
        if (this.parentId == 0) {
            this.newPost.parentPostId = undefined
        } else {
            let threadIds = this.pagedPosts.data.filter((p: Post) => p.id == this.parentId).map((p: Post) => p.inThreads.map((t: Thread) => t.id))[0]
            if (threadIds && threadIds.length > 0) {
                this.selectedThreads = this.allThreads.filter((t: Thread) => threadIds.includes(t.id))
            }
            this.newPost.parentPostId = this.parentId
        }
        this.newPost.createdByUserId = this.vm.loggedInUser.id
    }

    get btnText(): string {
        if (!this.post)
            return 'Skicka'
        return 'Spara'
    }
    get canSubmit() {
        return this.newPost.message ? this.newPost.message.length > 0 ? true : false : false
    }
    get inEditMode() {
        return this.post && this.post.inEdit ? true : false
    }
    get inReplyMode() {
        return this.parentId > 0 ? true : false
    }
    get maxNumberOfThreadsSelected() {
        return this.selectedThreads.length == 2 ? true : false
    }
    get allThreads() {
        return this.threads.filter((t: Thread) => t.type == ThreadType.Ongoing).concat(this.newThreads)
    }
    get threadsBtnText() {
        return this.showThreadSelection ? 'Dölj trådar' : this.selectedThreads.length <= 0 ? 'Tagga' : `${this.selectedThreads.length} tråd${this.selectedThreads.length > 1 ? 'ar' : ''}`
    }
    get threadsTooltip() {
        return this.postIsInPlannedThread ? 'Del av en planerad tråd. Kan inte ändra taggar.' : this.selectedThreads.length <= 0 ? 'Lägg till en tråd' : `Taggat med: ${this.selectedThreads.map((t: Thread) => t.name.toLowerCase()).join(', ')}`
    }
    get postIsInPlannedThread() {
        return this.post && this.post.inThreads.filter((t: Thread) => t.type == ThreadType.Planned).length > 0 ? true : false
    }

    disabledThreadSelection(name: string) {
        return this.selectedThreads.length == 2 && !this.selectedThreads.map((t: Thread) => t.name.toLowerCase()).includes(name.toLowerCase()) ? true : false
    }

    @Watch('results')
        onChange() {
            this.setShouldReloadPostStream({ value: true })
        }

    fetchLineups() {
        this.showLineupSelection = !this.showLineupSelection
        if (this.lineups.length <= 0) {
            this.loadLineups({ userId: this.vm.loggedInUser.id }).then(() => this.noLineupsFound = this.lineups.length == 0)
        }
    }
    
    changeLineup() {
		if (this.newPost.lineupId > 0) 
			this.setSelectedLineup(this.lineups.filter((l: Lineup) => l.id == this.newPost.lineupId)[0])
		else
			this.setSelectedLineup(new Lineup())
    }
    
    setCrudPostThreads() {
        this.newPost.threadIds = this.selectedThreads.filter((t: Thread) => t.id > 0).map((t: Thread) => t.id)
        this.newPost.newThreadNames = this.newThreads.length > 0 ? this.newThreads.map((t: Thread) => t.name.toLowerCase()) : undefined
        this.newPost.newThreadNames = this.newThreads.filter((nt: Thread) => this.selectedThreads.map((st: Thread) => st.name.toLowerCase()).includes(nt.name)).map((ft: Thread) => ft.name.toLowerCase())
    }

    submit() {
        if (this.inEditMode || this.inReplyMode && this.modalName) {
            this.$modal.hide(this.modalName)
        }
        this.setIsLoadingPosts({ value: true })
        if (!this.inEditMode && this.activeThread) {
            this.newPost.threadIds = [this.activeThread.id]
        } else {
            if (this.inEditMode) {
                if (this.post.inThreads.filter((t: Thread) => t.type == ThreadType.Planned).length <= 0) {
                    this.setCrudPostThreads()
                }
            } else {
                this.setCrudPostThreads()
            }
        }
        new Promise<Result[]>((resolve, reject) => {
            if (this.inEditMode) {
                this.update({ post: this.newPost }).then((res) => resolve(res)).catch(() => resolve([{ type: ResultType.Failure, message: 'Kunde inte uppdatera inlägget' }]))
            }
            else {
                this.create({ post: this.newPost }).then((res) => resolve(res)).catch(() => resolve([{ type: ResultType.Failure, message: 'Kunde inte svara på inlägget' }]))
            }
        }).then((res) => {
            this.results = res
            this.newPost = new CrudPost()
            this.newThreads = []
            this.selectedThreads = []
            this.showThreadSelection = false
            this.loadThreads()
            this.loadPaged({ pageNumber: 1, pageSize: this.vm.loggedInUser.filter.postsPerPage, filter: this.filter })
                .then(() => this.setIsLoadingPosts({ value: false }))
        })
    }

    addToSelections() {
        if (this.threads.map((t: Thread) => t.name).includes(this.newThreadName.toLocaleLowerCase())) {
            return
        }
        let thread: Thread = { id: 0, name: this.newThreadName.toLowerCase(), type: ThreadType.Ongoing, numberOfPosts: 0 }
        this.newThreads.push(thread)
        this.selectedThreads.push(thread)

        this.newThreadName = ''
    }
}
</script>

<style lang="scss" scoped>
.input-group:nth-child(2) {
    padding-top: 1.5rem;
    input, select, option {
        border-radius: 4px !important;
    }
}
</style>