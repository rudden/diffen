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

export default store