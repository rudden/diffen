<template>
    <div class="container">
        <div class="container-inner" v-if="!loading">
            <img class="rounded-circle media-object" :src="user.avatar" data-action="zoom">
            <template v-if="!inEdit">
                <a v-if="(isLoggedIn || loggedInUserIsAdmin)" v-on:click="inEdit = true" class="edit-btn">
                    <span class="icon icon-pencil"></span>
                </a>
                <h3 class="profile-header-user">{{ user.nickName }}</h3>
                <p class="profile-header-bio">{{ user.bio }}</p>
                <p v-if="user.inRoles && user.inRoles.length > 0">
                    <span class="badge badge-pill badge-light mr-1" v-for="role in user.inRoles">{{ role.toLowerCase() }}</span>
                </p>
                <p v-if="user.region">
                    <span class="icon icon-globe mr-2" style="color: green"></span>
                    <span class="badge badge-pill badge-primary text-uppercase">DIF {{ user.region }}</span>
                </p>
                <p v-if="user.favoritePlayer">
                    <span class="icon icon-heart mr-2" style="color: red"></span>
                    <span class="badge badge-pill badge-primary">{{ user.favoritePlayer.fullName }}</span>
                </p>
                <p v-if="user.secludedUntil">
                    <span class="badge badge-pill badge-danger">{{ `spärrad till ${user.secludedUntil}` }}</span>
                </p>
            </template>
            <template v-if="inEdit">
                <template v-if="isLoggedIn">
                    <div class="form-group">
                        <div class="custom-file" v-if="!hasSelectedAvatar">
                            <input type="file" class="custom-file-input" id="avatar" accept=".png,.jpg,.jpeg" @change="handleImageAdded" />
                            <label class="custom-file-label" for="avatar" style="text-align: left">välj en profilbild</label>
                        </div>
                        <div class="alert alert-primary" style="text-align: left" v-else>
                            <strong>{{ avatarFileName }}</strong>
                            <button type="button" class="close" aria-label="Close" v-on:click="deSelectFile">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                    </div>
                    <div class="form-group">
                        <input type="text" class="form-control" v-model="crudUser.nickName" placeholder="ditt nya nick" />
                    </div>
                    <div class="form-group">
                        <input type="text" class="form-control" v-model="crudUser.bio" placeholder="din nya bio" />
                    </div>
                    <div class="form-group">
                        <select class="form-control form-control-sm" v-model="selectedRegionName">
                            <option value="">välj ett område</option>
                            <option v-for="region in regions" :value="region.name">{{ region.name }}</option>
                        </select>
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
                    <template v-if="!selectedUserIsScissorOrAdmin">
                        <div class="form-group">
                            <date-picker v-model="secludeDate" :config="dpConfig" :placeholder="'spärra till datum'" />
                        </div>
                    </template>
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
            <results :items="results" />
        </div>
        <template v-else>
            <loader v-bind="{ background: '#699ED0' }" />
        </template>
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import axios from 'axios'
import { Component } from 'vue-property-decorator'
import { Getter, Action, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('profile', Getter)
const ModuleAction = namespace('profile', Action)

const SquadModuleGetter = namespace('squad', Getter)
const SquadModuleAction = namespace('squad', Action)

const OtherModuleGetter = namespace('other', Getter)
const OtherModuleAction = namespace('other', Action)

import { User } from '../../../model/profile/'
import { User as CrudUser } from '../../../model/profile/crud'
import { Player } from '../../../model/squad/'
import { Region } from '../../../model/other/'
import { PageViewModel, Result, ResultType } from '../../../model/common'

import Modal from '../../../components/modal.vue'
import Results from '../../../components/results.vue'

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

import {
    GET_REGIONS,
    FETCH_REGIONS
} from '../../../modules/other/types'

@Component({
    components: {
        Results, DatePicker, Modal
    }
})
export default class UserComponent extends Vue {
	@State(state => state.vm) vm: PageViewModel
	@ModuleGetter(GET_USER) user: User
	@ModuleAction(FETCH_USER) loadUser: (payload: { id: string }) => Promise<void>
	@ModuleAction(FETCH_ROLES) loadRoles: () => Promise<string[]>
    @ModuleAction(UPDATE_USER) updateUser: (payload: { userId: string, user: CrudUser }) => Promise<Result[]>

    @SquadModuleGetter(GET_PLAYERS) players: Player[]
    @SquadModuleAction(FETCH_PLAYERS) loadPlayers: () => Promise<void>
    
    @OtherModuleGetter(GET_REGIONS) regions: Region[]
	@OtherModuleAction(FETCH_REGIONS) loadRegions: () => Promise<void>

    inEdit: boolean = false
    loading: boolean = false

    roles: string[] = []
    crudUser: CrudUser = new CrudUser()
    favoritePlayerId: number = 0
    selectedRegionName: string = ''
    
    results: Result[] = []

    avatarFile: FormData = new FormData()
    avatarFileName: string = ''

    secludeDate: Date = new Date('')
    dpConfig: any = { 
		format: 'YYYY-MM-DD', 
		useCurrent: false, 
		locale: 'sv', 
		icons: { 
			next: 'icon icon-arrow-right',
			previous: 'icon icon-arrow-left' 
        },
        widgetPositioning: {
            vertical: 'bottom',
            horizontal: 'left'
        }
    }

    private directoryName: string = 'avatars'
    private genericAvatarSrc: string = '/uploads/avatars/generic/logo.png'

	created() {
        this.loadRegions()
        this.loadPlayers()

        if (this.loggedInUserIsAdmin) {
            this.loadRoles().then((roles: string[]) => this.roles = roles)
        }

        if (this.user.favoritePlayer) {
            this.favoritePlayerId = this.user.favoritePlayer.id
        }

        if (this.user.region) {
            this.selectedRegionName = this.user.region
        }

        if (this.user.secludedUntil) {
			this.secludeDate = new Date(this.user.secludedUntil)
		}

        this.setCrudUser()

        this.avatarFileName = !this.user.avatar.includes('generic/logo') ? this.user.avatar.split('_____')[1] : ''
    }
    
    get isLoggedIn(): boolean {
        return this.user.id == this.vm.loggedInUser.id
    }
	get loggedInUserIsAdmin(): boolean {
        return this.vm.loggedInUser.inRoles.some(role => role == 'Admin')
    }
    get selectedUserIsScissorOrAdmin(): boolean {
        return this.user.inRoles.some(role => role == 'Admin' || role == 'Scissor')
    }
    get canSave() {
		return this.crudUser.nickName && this.crudUser.nickName.length > 1 && this.crudUser.bio.length <= 100
    }
    get hasSelectedAvatar(): boolean {
        if (this.user.avatar != this.genericAvatarSrc) {
            return this.avatarFileName ? true : false
        }
        return this.avatarFileName ? true : false
    }
    
    setCrudUser() {
        this.crudUser = {
            nickName: this.user.nickName,
            bio: this.user.bio ? this.user.bio : '',
            roles: this.user.inRoles,
            favoritePlayerId: this.favoritePlayerId,
            region: this.selectedRegionName,
            secludeUntil: this.user.secludedUntil
        }
    }

	save() {
        this.loading = true
        this.crudUser.region = this.selectedRegionName
        this.crudUser.favoritePlayerId = this.favoritePlayerId
        if (this.secludeDate)
            if (isNaN(Date.parse(this.secludeDate.toString()))) {
                this.crudUser.secludeUntil = ''    
            } else {
                this.crudUser.secludeUntil = this.secludeDate.toString()
            }
        else {
            this.crudUser.secludeUntil = ''
        }
        this.updateUser({ userId: this.user.id, user: this.crudUser })
            .then((results: Result[]) => {
                this.results = results
                new Promise<void>((resolve, reject) => {
                    if (!this.avatarFileName) {
                        axios.delete(`${this.vm.api}/users/${this.vm.loggedInUser.id}/avatar`)
                            .then((res) => {
                                this.results.push(res.data)
                                this.user.avatar = this.genericAvatarSrc
                                resolve()
                            })
                    }
                    else if (this.user.avatar.split('_____')[1] !== this.avatarFileName)
                        this.uploadFile().then(() => resolve())
                    else resolve()
                }).then(() => {
                    this.loadUser({ id: this.user.id })
                        .then(() => {
                            this.setCrudUser()
                            this.inEdit = false
                            this.loading = false
                        })
                })
            })
    }

    uploadFile(): Promise<void> {
        return new Promise<void>((resolve, reject) => {
            axios.post(`${this.vm.api}/uploads/${this.directoryName}?uniqueId=${this.vm.loggedInUser.id}`, this.avatarFile, { headers: { 'Content-Type': 'application/json' } })
                .then((result) => {
                    let fileName: string = result.data
                    axios.post(`${this.vm.api}/users/${this.vm.loggedInUser.id}/avatar/${fileName}`)
                        .then((res) => {
                            this.results.push(res.data)
                            this.avatarFileName = fileName.split('_____')[1]
                            resolve()
                        })
                }).catch(() => {
                    resolve()
                    this.results.push({ type: ResultType.Failure, message: 'Kunde inte ladda upp den nya profilbilden. Trolig orsak är att den är för stor.' })
                })
        })
    }

    handleImageAdded(e: any) {
        var files = e.target.files
        if (!files.length)
            return
        this.avatarFile = new FormData()
        this.avatarFile.append('file', files[0])
        this.avatarFileName = files[0].name
    }

    deSelectFile() {
        this.avatarFile = new FormData()
        this.avatarFileName = ''
    }
}
</script>

<style lang="scss" scoped>
.edit-btn {
    // color: #699ED0 !important;
    color: white !important;
    font-size: 150%;
    cursor: pointer;
    display: block;
    margin-top: 20px;
}
</style>
