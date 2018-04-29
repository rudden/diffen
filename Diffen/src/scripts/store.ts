import Vue from 'vue'
import Vuex, { Module } from 'vuex'

import VModal from 'vue-js-modal'
import VueClipboards from 'vue-clipboards'

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

Vue.use(VueClipboards, store)
Vue.use(VModal, { componentName: "v-modal", dynamic: true, dialog: true }, store)

import Navbar from './components/navbar.vue'
Vue.component('navbar', Navbar)

import { Stretch } from 'vue-loading-spinner'
Vue.component('loader', Stretch)

export default store