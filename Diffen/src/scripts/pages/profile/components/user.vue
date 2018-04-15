<template>
    <div class="container">
        <div class="container-inner" v-if="!loading">
            <img class="rounded-circle media-object" :src="user.avatar">
            <a v-if="(isLoggedIn || loggedInUserIsAdmin) && !inEdit" v-on:click="inEdit = true" class="edit-btn">
                <span class="icon icon-pencil"></span>
            </a>
            <h3 class="profile-header-user">
                <span v-if="!inEdit">{{ user.nickName }}</span>
            </h3>
            <template v-if="!inEdit">
                <p class="profile-header-bio">{{ user.bio }}</p>
                <p v-if="user.inRoles && user.inRoles.length > 0">
                    <span class="badge badge-pill badge-light mr-1" v-for="role in user.inRoles">{{ role.toLowerCase() }}</span>
                </p>
                <p v-if="user.favoritePlayer">
                    <span class="badge badge-pill badge-primary">{{ user.favoritePlayer.fullName }}</span>
                </p>
                <seclude />
            </template>
            <template v-if="inEdit">
                <template v-if="isLoggedIn">
                    <div class="form-group">
                        <input type="text" class="form-control" v-model="crudUser.nickName" placeholder="ditt nya nick" />
                    </div>
                    <div class="form-group">
                        <input type="text" class="form-control" v-model="crudUser.bio" placeholder="din nya bio" />
                    </div>
                    <div class="form-group">
                        <select class="form-control form-control-sm" v-model="favoritePlayerId">
                            <option value="0">välj en spelare</option>
                            <option v-for="player in players" :value="player.id">{{ player.fullName }}</option>
                        </select>
                    </div>
                </template>
                <template v-if="roles && roles.length > 0 && loggedInUserIsAdmin">
                    <div class="form-group">
                        <div class="form-check form-check-inline" v-for="role in roles">
                            <input class="form-check-input" type="checkbox" :id="role" v-model="crudUser.roles" :value="role">
                            <label class="form-check-label" :for="role" style="color: white">{{ role.toLowerCase() }}</label>
                        </div>
                    </div>
                </template>
                <div class="form-group">
                    <div class="row">
                        <div class="col pr-1">
                            <button class="btn btn-sm btn-success btn-block" v-on:click="save" :disabled="!canSave">spara</button>
                        </div>
                        <div class="col pl-1">
                            <button class="btn btn-sm btn-danger btn-block" v-on:click="inEdit = false">avbryt</button>
                        </div>
                    </div>
                </div>
            </template>
            <results :items="results" :dismiss="dismiss" />
        </div>
        <template v-else>
            <loader v-bind="{ background: '#699ED0' }" />
        </template>
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Action, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('profile', Getter)
const ModuleAction = namespace('profile', Action)

const SquadModuleGetter = namespace('squad', Getter)
const SquadModuleAction = namespace('squad', Action)

import { User } from '../../../model/profile/'
import { User as CrudUser } from '../../../model/profile/crud'
import { Player } from '../../../model/squad/'
import { ViewModel, Result, ResultType } from '../../../model/common'

import Seclude from './seclude.vue'
import Modal from '../../../components/modal.vue'
import Results from '../../../components/results.vue'

import { Stretch as Loader } from 'vue-loading-spinner'
import DatePicker from 'vue-bootstrap-datetimepicker'

import {
    GET_USER,
    FETCH_USER,
    FETCH_ROLES,
    UPDATE_USER
} from '../../../modules/profile/types'

import {
    GET_PLAYERS,
    FETCH_PLAYERS
} from '../../../modules/squad/types'

@Component({
    components: {
        Results, Loader, DatePicker, Modal, Seclude
    }
})
export default class UserComponent extends Vue {
	@State(state => state.vm) vm: ViewModel
	@ModuleGetter(GET_USER) user: User
	@ModuleAction(FETCH_USER) loadUser: (payload: { id: string }) => Promise<void>
	@ModuleAction(FETCH_ROLES) loadRoles: () => Promise<string[]>
    @ModuleAction(UPDATE_USER) updateUser: (payload: { userId: string, user: CrudUser }) => Promise<Result[]>

    @SquadModuleGetter(GET_PLAYERS) players: Player[]
	@SquadModuleAction(FETCH_PLAYERS) loadPlayers: () => Promise<void>

    inEdit: boolean = false
    loading: boolean = false

    roles: string[] = []
    crudUser: CrudUser = new CrudUser()
    favoritePlayerId: number = 0
    
    results: Result[] = []

	created() {
        this.loadPlayers()
        this.loadRoles().then((roles: string[]) => this.roles = roles)

        if (this.user.favoritePlayer) {
            this.favoritePlayerId = this.user.favoritePlayer.id
        }

        this.setCrudUser()
    }
    
    get isLoggedIn(): boolean {
        return this.user.id == this.vm.loggedInUser.id
    }
	get loggedInUserIsAdmin(): boolean {
        return this.vm.loggedInUser.inRoles.some(role => role == 'Admin')
    }
    get canSave() {
		return this.crudUser.nickName && this.crudUser.nickName.length > 1 && this.crudUser.bio.length <= 100
    }
    
    setCrudUser() {
        this.crudUser = {
            nickName: this.user.nickName,
            bio: this.user.bio ? this.user.bio : '',
            roles: this.user.inRoles,
            favoritePlayerId: this.favoritePlayerId
        }
    }

	save() {
        this.loading = true
        this.crudUser.favoritePlayerId = this.favoritePlayerId
        this.updateUser({ userId: this.user.id, user: this.crudUser })
        .then((results: Result[]) => {
            this.results = results
            this.loadUser({ id: this.user.id })
                .then(() => {
                    this.setCrudUser()
                    this.inEdit = false
                    this.loading = false
                })
        })
    }

    dismiss(type: ResultType) {
		this.results = this.results.filter((r: Result) => r.type != type)
	}
}
</script>

<style lang="scss" scoped>
.edit-btn {
    color: #699ED0 !important;
    font-size: 150%;
    cursor: pointer;
    display: block;
    margin-top: 20px;
}
</style>
