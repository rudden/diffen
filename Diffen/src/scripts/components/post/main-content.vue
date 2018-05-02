<template>
    <div>
        <div class="media-heading">
            <small class="float-right text-muted">{{ post.since }}</small>
            <h6>
                <a :href="`/profil/${post.user.id}`">{{ post.user.nickName }}</a>
                <template v-if="post.updated">
                    <span class="badge badge-danger ml-2">editerat</span>
                </template>
            </h6>
        </div>
        <p>{{ post.message }}</p>
        <embeds :href="post.urlTipHref" />
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'

import { Post } from '../../model/forum'

import Embeds from './embeds.vue'

@Component({
    props: {
        post: Object,
    },
    components: {
        Embeds
    }
})
export default class PostMainContent extends Vue {
    post: Post
}
</script>

<style lang="scss" scoped>
a {
    color: black;
}
</style>
