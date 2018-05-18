<template>
	<div class="col-lg-3" v-show="showRightSideBar">
		<!-- <polls-component :type-of-polls="'active'" :is-small="true" class="mb-4" />
		<chronicles-component :is-small="true" :amount-of-chronicles="5" class="mb-4" /> -->
		<rss-feed :url="'https://www.jarnkaminerna.se/feed/'" :feed-name="'Senaste från Järnkaminerna'" />
		<rss-feed :url="'https://diftv.solidtango.com/feed/'" :feed-name="'Senaste från DIFTV'" />
		<div class="card mb-4">
            <div class="card-body">
                <h6 class="mb-3">Länktipstoppen</h6>
                <ul class="list-unstyled list-spaced mb-0" v-if="urlTips.length > 0">
                    <li class="ellipsis" v-for="tip in urlTips" :key="tip.href + tip.clicks">
                        <span class="text-muted icon" :class="icon(tip.href)"></span>
                        <small class="ml-1"><span class="badge badge-secondary">{{ tip.clicks }}</span></small>
                        <url :tip="tip" class="ml-1" />
                    </li>
                </ul>
            </div>
        </div>
		<div class="card card-link-list">
			<div class="card-body">
				© {{ currentYear }} Blåränderna
			</div>
		</div>
	</div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Action, Mutation, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('forum', Getter)
const ModuleAction = namespace('forum', Action)
const ModuleMutation = namespace('forum', Mutation)

import { GET_URLTIP_TOPLIST, GET_SHOW_RIGHT_SIDEBAR, FETCH_URLTIP_TOPLIST, SET_SHOW_RIGHT_SIDEBAR } from '../../../modules/forum/types'

import { PageViewModel } from '../../../model/common'
import { UrlTip } from '../../../model/forum'

import FilterComponent from './filter.vue'
import PollsComponent from '../../../components/other/polls.vue'
import ChroniclesComponent from '../../../components/other/chronicles.vue'
import RssFeed from '../../../components/rss.vue'
import Url from '../../../components/url.vue'

@Component({
	components: {
		FilterComponent, PollsComponent, ChroniclesComponent, RssFeed, Url
	}
})
export default class RightSideBar extends Vue {
    @State(state => state.vm) vm: PageViewModel
	@ModuleGetter(GET_SHOW_RIGHT_SIDEBAR) showRightSideBar: boolean
    @ModuleGetter(GET_URLTIP_TOPLIST) urlTips: UrlTip[]
	@ModuleAction(FETCH_URLTIP_TOPLIST) loadUrlTipTopList: () => Promise<void>
    @ModuleMutation(SET_SHOW_RIGHT_SIDEBAR) setShowRightSideBar: (payload: { value: boolean }) => void
	
	currentYear: number = (new Date()).getFullYear()

	mounted() {
        this.loadUrlTipTopList()
        this.setShowRightSideBar({ value: !this.vm.loggedInUser.filter.hideRightMenu })
	}

	
    icon(href: string): string {
        if (href.includes('facebook'))
            return 'icon-facebook'
        if (href.includes('instagram'))
            return 'icon-instagram'
        if (href.includes('twitter'))
            return 'icon-twitter'
        if (href.includes('youtube'))
            return 'icon-youtube'
        if (href.includes('spotify'))
            return 'icon-spotify'
        if (href.includes('soundcloud'))
            return 'icon-soundcloud'
        if (href.includes('dif'))
            return 'icon-heart'
        if (href.includes('aik.se') || href.includes('aikfotboll.se'))
            return 'icon-trash'
        return 'icon-link'
    }
}
</script>