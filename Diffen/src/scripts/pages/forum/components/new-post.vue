<template>
    <div>
        <div class="input-group">
            <textarea rows="5" class="form-control" placeholder="ditt inlägg.." v-model="newPost.message"></textarea>
        </div>
        <div class="input-group">
            <input type="text" class="form-control" placeholder="länktips" v-model="newPost.urlTip.href">
            <div class="input-group-btn">
                <button class="btn btn-success align-self-stretch ml-2" v-on:click="submit" :disabled="!canSubmit">{{ btnText }}</button>
            </div>
        </div>
        <results :items="results" />
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Mutation, Action, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('forum', Getter)
const ModuleAction = namespace('forum', Action)
const ModuleMutation = namespace('forum', Mutation)

import { CREATE_POST, UPDATE_POST, SET_IS_LOADING_POSTS } from '../../../modules/forum/types'

import { Post } from '../../../model/forum'
import { Post as CrudPost } from '../../../model/forum/crud'
import { ViewModel, Result } from '../../../model/common'

import Results from '../../../components/results.vue'

@Component({
    props: {
        post: Object,
        parentId: {
            type: Number,
            default: 0
        }
    },
    components: {
        Results
    }
})
export default class NewPost extends Vue {
  	@State(state => state.vm) vm: ViewModel
    @ModuleAction(CREATE_POST) create: (payload: { post: CrudPost }) => Promise<Result[]>
    @ModuleAction(UPDATE_POST) update: (payload: { post: CrudPost }) => Promise<Result[]>
	@ModuleMutation(SET_IS_LOADING_POSTS) setIsLoadingPosts: (payload: { value: boolean }) => void

    post: Post
    parentId: number

    loading: boolean = true
    newPost: CrudPost = new CrudPost()
    results: Result[] = []

    created() {
        if (this.post) {
            this.newPost.id = this.post.id
            this.newPost.message = this.post.message
            this.newPost.urlTip.href = this.post.urlTipHref
        }
        this.newPost.parentPostId = this.parentId
        this.newPost.createdByUserId = this.vm.loggedInUser.id
    }
    
    get btnText(): string {
        if (!this.post)
            return 'skapa inlägg'
        return 'spara inlägg'
    }
    get canSubmit() {
        return this.newPost.message ? this.newPost.message.length > 0 ? true : false : false
    }

    submit() {
        this.setIsLoadingPosts({ value: true })
        new Promise<Result[]>((resolve, reject) => {
            if (this.newPost.id > 0) {
                this.update({ post: this.newPost }).then((res) => resolve(res))
            }
            else {
                this.create({ post: this.newPost })
                    .then((res) => {
                        resolve(res)
                        this.newPost = new CrudPost()
                    })
            }
        }).then((res) => {
            this.results = res
            this.setIsLoadingPosts({ value: false })
        })

    }
}
</script>

<style lang="scss" scoped>
.input-group:nth-child(2) {
    padding-top: 1.5rem;
    input {
        border-radius: 4px !important;
    }
}
</style>
