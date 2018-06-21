<template>
    <div class="wrap float-right">
        <template v-if="post.loggedInUserCanVote">
            <modal v-bind="{ attributes: { name: `vote-up-${post.id}` }, header: 'Vill du rösta upp inlägget?', button: { classes: 'small-device small-device__contents', icon: 'icon icon-thumbs-up' } }">
                <template slot="body">
                    <div class="row">
                        <div class="col">
                            <button class="btn btn-sm btn-success btn-block" v-on:click="createVote(upvoteType)">Ja! <span class="icon icon-thumbs-up"></span></button>
                        </div>
                    </div>
                </template>
            </modal>
            <modal v-bind="{ attributes: { name: `vote-down-${post.id}` }, header: 'Vill du rösta ner inlägget?', button: { classes: 'small-device small-device__contents', icon: 'icon icon-thumbs-down' } }">
                <template slot="body">
                    <div class="row">
                        <div class="col">
                            <button class="btn btn-sm btn-success btn-block" v-on:click="createVote(downvoteType)">Ja! <span class="icon icon-thumbs-down"></span></button>
                        </div>
                    </div>
                </template>
            </modal>
            <a v-on:click="createVote(upvoteType)" class="large-device large-device__flex">
                <span class="icon icon-thumbs-up" :class="{ 'icon-thumbs-up__green': hasUpvoted }"></span>
            </a>
            <a v-on:click="createVote(downvoteType)" class="large-device large-device__flex">
                <span class="icon icon-thumbs-down" :class="{ 'icon-thumbs-down__red': hasDownvoted }"></span>
            </a> · 
        </template>
        <template v-if="post.votes.length > 0">
            <a :id="'votes-' + post.id">
                <span class="badge badge-secondary" style="margin-top: 3px;">{{ karma }}</span>
            </a>
            <popover title="tummar" :target="'#votes-' + post.id">
                <template slot="popover">
                    <div class="votes">
                        <div v-for="vote in post.votes" :key="vote.id">
                            <span class="icon icon-thumbs-up" v-show="vote.type == upvoteType"></span>
                            <span class="icon icon-thumbs-down" v-show="vote.type == downvoteType"></span>
                            <span class="mr-2 ml-2">{{ vote.byNickName }}</span>
                        </div>
                    </div>
                </template>
            </popover>
        </template>
        <template v-else>
            <span class="badge badge-secondary" style="margin-top: 3px;">{{ karma }}</span>
        </template>
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Action, State, namespace } from 'vuex-class'

const ModuleAction = namespace('forum', Action)

import { CREATE_VOTE, DELETE_VOTE } from '../../modules/forum/types'

import { Post, VoteType, Vote } from '../../model/forum'
import { Vote as CrudVote } from '../../model/forum/crud'
import { PageViewModel } from '../../model/common'

import Modal from '../modal.vue'

import { Popover } from 'uiv'

@Component({
    props: {
        post: Object
    },
    components: {
        Popover, Modal
    }
})
export default class Voting extends Vue {
    @State(state => state.vm) vm: PageViewModel
    @ModuleAction(CREATE_VOTE) vote: (payload: { vote: CrudVote }) => Promise<void>
    @ModuleAction(DELETE_VOTE) removeVote: (payload: { id: number }) => Promise<boolean>

    post: Post

    get upvoteType() { return VoteType.Up }
    get downvoteType() { return VoteType.Down }

    get currentVoteId() {
        if (this.hasUpvoted)
            return this.upvote.id
        if (this.hasDownvoted)
            return this.downvote.id
        return 0
    }

    get upvote() {
        return this.post.votes.filter(v => v.type == this.upvoteType && v.byNickName == this.vm.loggedInUser.nickName)[0]
    }
    get downvote() {
        return this.post.votes.filter(v => v.type == this.downvoteType && v.byNickName == this.vm.loggedInUser.nickName)[0]
    }

    get hasUpvoted() {
        return this.upvote !== undefined
    }
    get hasDownvoted() {
        return this.downvote !== undefined
    }

    get karma(): number {
        let karma: number = 0
        this.post.votes.forEach((v: Vote) => {
            switch (v.type) {
                case VoteType.Up:
                    karma++
                    break
                case VoteType.Down:
                    karma--
                    break
            }
        })
        return karma
    }

    createVote(type: VoteType) {
        let payload = {
            type: type,
            postId: this.post.id,
            createdByUserId: this.vm.loggedInUser.id
        }
        switch (type) {
            case VoteType.Up:
                if (this.hasUpvoted) {
                   this.deleteVote()
                } else {
                    if (this.hasDownvoted) {
                        this.deleteVote().then(() => this.vote({ vote: payload }).then(() => this.$forceUpdate()))
                    }
                    this.vote({ vote: payload }).then(() => this.$forceUpdate())
                }
                break
            case VoteType.Down:
                if (this.hasDownvoted) {
                    this.deleteVote()
                } else {
                    if (this.hasUpvoted) {
                        this.deleteVote().then(() => this.vote({ vote: payload }).then(() => this.$forceUpdate()))
                    }
                    this.vote({ vote: payload }).then(() => this.$forceUpdate())
                }
                break
        }
    }

    deleteVote() {
        return this.removeVote({ id: this.currentVoteId })
            .then((res) => {
                if (res) {
                    this.post.votes = this.post.votes.filter(v => v.id !== this.currentVoteId)
                    this.$forceUpdate()
                }
            })
    }
}
</script>

<style lang="scss" scoped>
.icon-thumbs-down {
    &__red {
        color: red !important;
    }
}
.icon-thumbs-up {
    &__green {
        color: green !important;
    }
}
</style>
