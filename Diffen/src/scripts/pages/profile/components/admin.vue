<template>
	<div class="container container__profile mt-3 mb-5">
		<div class="card">
			<div class="card-header">Administration</div>
			<div class="card-body">
                <player-events-admin />
			</div>
		</div>
	</div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Mutation, Action, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('profile', Getter)
const ModuleAction = namespace('profile', Action)
const ModuleMutation = namespace('profile', Mutation)

import PlayerEventsAdmin from '../../../components/player-events-admin.vue'

@Component({
	components: {
		PlayerEventsAdmin
	}
})
export default class Admin extends Vue {

}
</script>