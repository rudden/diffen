import State from './state';
import Mutations from './mutations';
import Getters from './getters';
import Actions from './actions';
export var Forum = {
    namespaced: true,
    state: new State(),
    getters: Getters,
    actions: Actions,
    mutations: Mutations
};
export default Forum;
