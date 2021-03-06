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
Vue.use({
    install () {
        Vue.prototype.$helpers = helpers
    }
})

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

import 'chart.js'
import 'hchs-vue-charts'
Vue.use((<any>window).VueCharts)

import TableComponent from 'vue-table-component'

Vue.use(TableComponent, {
    tableClass: 'table table-bordered table-hover table-responsive-lg table-sm mb-0',
    theadClass: 'thead-dark',
    tbodyClass: '',
    filterPlaceholder: 'Sök efter en spelare',
	filterNoResults: 'Hittade inget',
	filterInputClass: 'form-control mb-3'
})

import DatePicker from 'vue-bootstrap-datetimepicker'
Vue.component('date-picker', DatePicker)

import Datepicker from 'vuejs-datepicker'
Vue.component('v-datepicker', Datepicker)

export default store