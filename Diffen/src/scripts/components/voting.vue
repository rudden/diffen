<template>
    <div class="wrap float-right">
        <template v-if="post.loggedInUserCanVote">
            <a v-on:click="createVote(upvoteType)">
                <span class="icon icon-thumbs-up"></span>
            </a>
            <a v-on:click="createVote(downvoteType)">
                <span class="icon icon-thumbs-down"></span>
            </a> · 
        </template>
        <template v-if="post.votes.length > 0">
            <a :id="'votes-' + post.id">
                <span class="badge badge-secondary" style="margin-top: 3px;">{{ karma }}</span>
            </a>
            <popover title="tummar" :target="'#votes-' + post.id">
                <template slot="popover">
                    <div class="votes">
                        <div v-for="vote in post.votes">
                            <span class="icon icon-thumbs-up" v-show="vote.type == upvoteType"></span>
                            <span class="icon icon-thumbs-down" v-show="vote.type == downvoteType"></span>
                            <span class="mr-2">{{ vote.byNickName }}</span>
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

import { CREATE_VOTE } from '../modules/forum/types'

import { Post, VoteType, Vote } from '../model/forum'
import { Vote as CrudVote } from '../model/forum/crud'
import { ViewModel } from '../model/common'

import { Popover } from 'uiv'

@Component({
    props: {
        post: Object
    },
    components: {
        Popover
    }
})
export default class Voting extends Vue {
    @State(state => state.vm) vm: ViewModel
    @ModuleAction(CREATE_VOTE) vote: (payload: { vote: CrudVote }) => Promise<void>

    post: Post

    get upvoteType() { return VoteType.Up }
    get downvoteType() { return VoteType.Down }

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
        this.vote({ vote: { type: type, postId: this.post.id, createdByUserId: this.vm.loggedInUser.id } })
            .then(() => this.$forceUpdate())
    }
}
</script>

<style lang="scss" scoped>
.icon-thumbs-up {
    color: green;
}
.icon-thumbs-down {
    color: red;
}
</style>
