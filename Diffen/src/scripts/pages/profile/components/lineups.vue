<template>
	<div>
		<h6>startelvor</h6>
	</div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Mutation, Action, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('profile', Getter)
const ModuleAction = namespace('profile', Action)
const ModuleMutation = namespace('profile', Mutation)

import { ViewModel } from '../../../model/common'

import { GET_USER, FETCH_USER } from '../../../modules/profile/types'

@Component({})
export default class Lineups extends Vue {
	@State(state => state.vm) vm: ViewModel
}
</script>