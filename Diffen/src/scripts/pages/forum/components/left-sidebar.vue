<template>
    <div class="col-lg-3">
        <div class="card card-profile mb-4">
            <div class="card-header" style="background-image: url(stadion.png);"></div>
            <div class="card-body text-center">
                <a href="/profile">
                    <img class="card-profile-img" :src="vm.loggedInUser.avatar">
                </a>
                <h6 class="card-title">
                    <a class="text-inherit" href="/profile">{{ vm.loggedInUser.nickName }}</a>
                </h6>
                <p class="mb-4">{{ vm.loggedInUser.bio }}</p>
                <ul class="card-menu">
                    <li class="card-menu-item">
                        inlägg
                        <h6 class="my-0">{{ vm.loggedInUser.numberOfPosts }}</h6>
                    </li>
                    <li class="card-menu-item">
                        karma
                        <h6 class="my-0">{{ vm.loggedInUser.karma }}</h6>
                    </li>
                </ul>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { State } from 'vuex-class'

import { Post, Vote, VoteType } from '../../../model/forum'
import { ViewModel } from '../../../model/common'

@Component({})
export default class LeftSidebar extends Vue {
    @State(state => state.vm) vm: ViewModel
}
</script>