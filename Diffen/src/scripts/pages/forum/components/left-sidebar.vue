<template>
    <div class="col-lg-3 d-none d-lg-block" v-if="showLeftSideBar">
        <div class="card card-profile mb-4">
            <div class="card-header" style="background-image: url(/bg.jpg);"></div>
            <div class="card-body text-center">
                <a href="/profil">
                    <img class="card-profile-img mb-3" :src="user.avatar">
                </a>
                <p class="mb-4" v-if="user.bio">{{ user.bio }}</p>
                <ul class="card-menu mb-0">
                    <li class="card-menu-item">
                        Inlägg
                        <h6 class="my-0">{{ user.numberOfPosts }}</h6>
                    </li>
                    <li class="card-menu-item">
                        Karma
                        <h6 class="my-0">{{ user.karma }}</h6>
                    </li>
                </ul>
            </div>
        </div>
        <div class="card mb-4">
            <div class="card-body">
                <h6 class="card-title">
                    <a class="text-inherit" href="/profil">{{ user.nickName }}</a>
                </h6>
                <hr />
                <ul class="list-unstyled list-spaced mb-0">
                    <li><span class="text-muted icon icon-thumbs-up mr-3"></span>{{ user.voteStatistics.upVotes }} givna upptummar</li>
                    <li><span class="text-muted icon icon-thumbs-down mr-3"></span>{{ user.voteStatistics.downVotes }} givna nertummar</li>
                    <template v-if="user.savedPostsIds">
                        <li><span class="text-muted icon icon-bookmark mr-3"></span>{{ user.savedPostsIds.length }} sparade inlägg</li>
                    </template>
                    <template v-if="user.favoritePlayer">
                        <li><span class="text-muted icon icon-heart mr-3"></span>{{ user.favoritePlayer.fullName }}</li>
                    </template>
                    <template v-if="user.region">
                        <li class="text-uppercase"><span class="text-muted icon icon-globe mr-3"></span>DIF {{ user.region }}</li>
                    </template>
                </ul>
            </div>
        </div>
        <game-guesser />
        <rss-feed :url="'https://www.jarnkaminerna.se/feed/'" :amount="5" :feed-name="'Senaste från Järnkaminerna'" />
		<rss-feed :url="'https://diftv.solidtango.com/feed/'" :amount="5" :feed-name="'Senaste från DIFTV'" />
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Action, Mutation, State, namespace } from 'vuex-class'

import { PageViewModel } from '../../../model/common'

const ModuleGetter = namespace('forum', Getter)
const ModuleAction = namespace('forum', Action)
const ModuleMutation = namespace('forum', Mutation)

import { GET_SHOW_LEFT_SIDEBAR, SET_SHOW_LEFT_SIDEBAR } from '../../../modules/forum/types'

import GameGuesser from '../../../components/game-guesser.vue'
import RssFeed from '../../../components/rss.vue'

@Component({
    components: {
        RssFeed, GameGuesser
    }
})
export default class LeftSidebar extends Vue {
    @State(state => state.vm) vm: PageViewModel
    @ModuleGetter(GET_SHOW_LEFT_SIDEBAR) showLeftSideBar: boolean
    @ModuleMutation(SET_SHOW_LEFT_SIDEBAR) setShowLeftSideBar: (payload: { value: boolean }) => void

    mounted() {
        this.setShowLeftSideBar({ value: !this.vm.loggedInUser.filter.hideLeftMenu })
    }

    get user() {
        return this.vm.loggedInUser
    }
}
</script>
