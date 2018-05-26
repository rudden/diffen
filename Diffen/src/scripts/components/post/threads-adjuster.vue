<template>
    <div>
        <template v-if="!loading">
            <div class="row">
                <div class="col">
                    <div class="form-check form-check-inline" v-for="thread in threads" :key="thread.id">
                        <input class="form-check-input" type="checkbox" :id="thread.id" :value="thread.id" v-model="selectedThreads" :disabled="disabledThreadSelection(thread.id)">
                        <label class="form-check-label" :for="thread.id">{{ thread.name }}</label>
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
            <results :items="results" class="mt-3" />
            <div class="row mt-3">
                <div class="col">
                    <button class="btn btn-sm btn-success btn-block" :disabled="!canSubmit" @click="submit">Spara</button>
                </div>
            </div>
        </template>
        <template v-else>
            <loader v-bind="{ background: '#699ED0' }" />
        </template>
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Mutation, Action, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('forum', Getter)
const ModuleAction = namespace('forum', Action)
const ModuleMutation = namespace('forum', Mutation)

import { GET_THREADS, UPDATE_THREADS_ON_POST } from '../../modules/forum/types'

import { Post, Thread, ThreadType } from '../../model/forum'
import { Post as CrudPost } from '../../model/forum/crud'
import { PageViewModel, Result, ResultType } from '../../model/common'

import Results from '../results.vue'

@Component({
    props: {
        post: Object
    },
    components: {
        Results
    }
})
export default class ThredsAdjuster extends Vue {
  	@State(state => state.vm) vm: PageViewModel
    @ModuleGetter(GET_THREADS) threads: Thread[]
    @ModuleAction(UPDATE_THREADS_ON_POST) updateThreads: (payload: { postId: number, threadIds: number[] }) => Promise<boolean>

    post: Post

    loading: boolean = false

    selectedThreads: number[] = []

    results: Result[] = []

    created() {
        this.selectedThreads = this.threads.filter((t: Thread) => this.post.inThreads.map((t2: Thread) => t2.id).includes(t.id)).map((t: Thread) => t.id)
    }

    get canSubmit() {
        return this.selectedThreads.length <= 2 ? true : false
    }
    get maxNumberOfThreadsSelected() {
        return this.selectedThreads.length == 2 ? true : false
    }
    get postIsInPlannedThread() {
        return this.post && this.post.inThreads.filter((t: Thread) => t.type == ThreadType.Planned).length > 0 ? true : false
    }

    disabledThreadSelection(id: number) {
        return this.selectedThreads.length == 2 && !this.selectedThreads.includes(id) ? true : false
    }

    submit() {
        this.loading = true
        this.updateThreads({ postId: this.post.id, threadIds: this.selectedThreads })
            .then((result: boolean) => {
                if (result) {
                    this.results.push({ type: ResultType.Success, message: 'Uppdaterade inläggets trådar' })
                } else {
                    this.results.push({ type: ResultType.Failure, message: 'Något gick fel när trådarna skulle uppdateras' })
                }
                this.loading = false
            }).catch(() => this.results.push({ type: ResultType.Failure, message: 'Något gick fel när trådarna skulle uppdateras' }))
    }
}
</script>