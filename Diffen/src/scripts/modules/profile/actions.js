import axios from 'axios';
// import { Result } from '../../model/common'
import { FETCH_USER, FETCH_KVP_USERS, SET_USER, SET_KVP_USERS } from './types';
// export everything compliant to the vuex specification for actions
export var Actions = (_a = {},
    _a[FETCH_USER] = function (store, payload) {
        return axios.get(store.rootState.vm.api + "/users/" + payload.id)
            .then(function (res) { return store.commit(SET_USER, res.data); }).catch(function (error) { return console.warn(error); });
    },
    _a[FETCH_KVP_USERS] = function (store) {
        return axios.get(store.rootState.vm.api + "/users")
            .then(function (res) { return store.commit(SET_KVP_USERS, res.data); }).catch(function (error) { return console.warn(error); });
    },
    _a);
export default Actions;
var _a;
