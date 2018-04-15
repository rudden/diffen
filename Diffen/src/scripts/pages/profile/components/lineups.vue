<template>
	<div class="container container__profile mt-3 mb-5">
		<lineup-component />
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

import LineupComponent from '../../../components/lineups/lineups.vue'

@Component({
	components: {
		LineupComponent
	}
})
export default class Lineups extends Vue {
	@State(state => state.vm) vm: ViewModel
}
</script>