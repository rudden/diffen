<template>
    <div class="card mb-4 mb-4" v-if="!notFound">
        <div class="card-body">
            <h6 class="mb-3">{{ feedName }}</h6>
            <hr />
            <template v-if="!loading">
                <ul class="list-unstyled list-spaced mb-0" v-if="items.length > 0">
                    <li v-for="item in items" :key="item.id">
                        <strong>
                            <small>
                                <a target="_blank" :href="item.link">{{ item.title }}</a>
                            </small>
                        </strong>
                    </li>
                </ul>
                <div class="alert alert-warning mb-0" v-else>
                    Kunde inte hämta inlägg från rss {{ url }}
                </div>
            </template>
            <template v-else>
                <loader v-bind="{ background: '#699ED0' }" />
            </template>
        </div>
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'

import Parser from 'rss-parser'

interface RssItem {
    id: number
    title: string
    link: string
}

@Component({
    props: {
        url: String,
        feedName: String
    }
})
export default class RssParser extends Vue {
    url: string
    feedName: string

    items: RssItem[] = []
    loading: boolean = true
    notFound: boolean = false

    private corsBaseUrl: string = 'https://cors.now.sh/'

	mounted() {
        this.fetch().then(() => this.loading = false).catch(() => this.notFound = true)
    }
    
    fetch() {
        return new Promise<void>((resolve, reject) => {
            new Parser().parseURL(`${this.corsBaseUrl}${this.url}`, (err: any, feed: any) => {
                if (feed) {
                    for (let i = 0; i < feed.items.length; i++) {
                        this.items.push({
                            id: i,
                            title: feed.items[i].title,
                            link: feed.items[i].link
                        })
                    }
                    resolve()
                }
                reject()
            })
        })
    }
}
</script>

<style lang="scss" scoped>
li {
    overflow: hidden;
    white-space: nowrap;
    text-overflow: ellipsis;
}
</style>