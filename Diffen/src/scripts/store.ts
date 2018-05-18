import Vue from 'vue'
import Vuex, { Module } from 'vuex'

import Forum from './modules/forum'
import Profile from './modules/profile'
import Squad from './modules/squad'
import Other from './modules/other'

import { PageViewModel } from './model/common'

Vue.use(Vuex)

interface State {
	vm: PageViewModel
}
const state: State = {
	vm: new PageViewModel()
}

const store = new Vuex.Store({
	state: state,
	modules: {
		forum: <Module<any, State>> Forum,
		profile: <Module<any, State>> Profile,
		squad: <Module<any, State>> Squad,
		other: <Module<any, State>> Other
	}
})

import * as helpers from './helpers'
const helperMethodsPlugin = {
    install () {
        Vue.prototype.$helpers = helpers
    }
}
Vue.use(helperMethodsPlugin)

import VueClipboards from 'vue-clipboards'
Vue.use(VueClipboards, store)

import VModal from 'vue-js-modal'
Vue.use(VModal, { componentName: "v-modal", dynamic: true, dialog: true }, store)

import Navbar from './components/navbar.vue'
Vue.component('navbar', Navbar)

import { Stretch } from 'vue-loading-spinner'
Vue.component('loader', Stretch)

import Multiselect from 'vue-multiselect'
Vue.component('v-multiselect', Multiselect)

import ToggleButton from 'vue-js-toggle-button'
Vue.use(ToggleButton)

import VTooltip from 'v-tooltip'
Vue.use(VTooltip)

export default store