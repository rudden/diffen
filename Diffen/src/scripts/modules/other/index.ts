import { Module } from 'vuex'
import State from './state'
import Mutations from './mutations'
import Getters from './getters'
import Actions from './actions'

export const Other: Module<State, any> = {
	namespaced: true,

	state: new State(),
	getters: Getters,
	actions: Actions,
	mutations: Mutations
}

export default Other