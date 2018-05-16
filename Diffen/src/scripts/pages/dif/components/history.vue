<template>
    <div>
        <h6>Kommer snart!</h6>
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Action, State, namespace } from 'vuex-class'

const ModuleAction = namespace('squad', Action)

import { Title, TitleType } from '../../../model/squad'
import { PageViewModel } from '../../../model/common'

import { FETCH_TITLES } from '../../../modules/squad/types'

import Modal from '../../../components/modal.vue'

@Component({
	components: {
		Modal
	}
})
export default class History extends Vue {
	@State(state => state.vm) vm: PageViewModel

    loading: boolean = true

	mounted() {
	}

	get loggedInUserIsAdmin(): boolean {
        return this.vm.loggedInUser.inRoles.some(role => role == 'Admin')
    }
}
</script>