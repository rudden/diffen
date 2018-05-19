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
        <template v-if="post.parentPost && showParent">
            <div class="card mt-3 mb-3 parent-item">
                <div class="card-body">
                    <div class="media">
                        <div class="media-body">
                            <div class="media-body-text">
                                <div class="media-heading">
                                    <small class="float-right text-muted">{{ post.parentPost.since }}</small>
                                    <h6>{{ post.parentPost.user.nickName }}</h6>
                                </div>
                                <span class="message more-readable">{{ post.parentPost.message }}</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </template>
        <p v-html="postMessage" class="more-readable"></p>
        <embeds :href="post.urlTipHref" />
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, namespace } from 'vuex-class'

const ModuleGetter = namespace('forum', Getter)

import { GET_FILTER } from '../../modules/forum/types'

import { Post, Filter } from '../../model/forum'

import Embeds from './embeds.vue'

@Component({
    props: {
        post: Object,
        showParent: {
            type: Boolean,
            default: true
        } 
    },
    components: {
        Embeds
    }
})
export default class PostMainContent extends Vue {
    @ModuleGetter(GET_FILTER) filter: Filter

    post: Post

    get postMessage() {
        return this.filterMessage(this.post.message)
    }

    get parentPostMessage() {
        return this.filterMessage(this.post.parentPost.message)
    }

    filterMessage(message: string) {
        if (this.filter.messageWildCard && message) {
            var pattern = new RegExp(this.filter.messageWildCard, 'gi');
            return message.replace(pattern, '<strong><u>' + this.filter.messageWildCard + '</u></strong>')
        }
        return message
    }
}
</script>

<style lang="scss" scoped>
.parent-item {
    background-color: #f5f8fa;
    .media-body-text span {
        white-space: pre-wrap;
    }
}
</style>
