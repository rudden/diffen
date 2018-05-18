<template>
    <modal v-bind="{
        attributes: {
                name: 'new-poll'
            },
            header: 'Ny omröstning',
            button: {
                icon: 'icon icon-plus float-right',
                text: 'Skapa ny omröstning'
            },
            onClose: onModalClose
        }">
        <template slot="body">
            <template v-if="creating">
                <loader v-bind="{ background: '#699ED0' }" />
            </template>
            <template v-else>
                <div class="row">
                    <div class="col">
                        <div class="form-group row">
                            <label for="name" class="col-sm-2 col-form-label">Titel</label>
                            <div class="col-sm-10">
                                <input type="text" id="name" v-model="newPoll.name" class="form-control form-control-sm" placeholder="Namn på omröstningen" />
                            </div>
                        </div>
                        <div class="form-group row">
                            <label for="selection" class="col-sm-2 col-form-label">Alternativ</label>
                            <div class="col-sm-10">
                                <input type="text" id="selection" v-model="newSelection" class="form-control form-control-sm" :placeholder="selectionPlaceholder" v-on:keyup.enter="addToSelections" :disabled="maxNumberOfSelections" />
                            </div>
                        </div>
                        <div class="form-group">
                        </div>
                        <template v-if="newPoll.selections.length > 0">
                            <hr />
                            <ul class="list-unstyled list-spaced mb-3">
                                <li><strong>Alternativ</strong></li>
                                <li v-for="selection in newPoll.selections" :key="selection">
                                    <a class="float-right" v-on:click="removeSelection(selection)">
                                        <span class="icon icon-trash"></span>
                                    </a>
                                    <span>{{ selection }}</span>
                                </li>
                            </ul>
                        </template>
                    </div>
                </div>
                <results :items="results" class="pb-3" />
                <div class="row">
                    <div class="col">
                        <button class="btn btn-success btn-sm btn-block" v-on:click="create" :disabled="!canCreate">Skapa</button>
                    </div>
                </div>
            </template>
        </template>
    </modal>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Action, State, namespace } from 'vuex-class'

const ModuleAction = namespace('other', Action)

import { PageViewModel, Result, ResultType } from '../../model/common'
import { Poll, PollSelection } from '../../model/other'
import { Poll as CrudPoll } from '../../model/other/crud'

import { CREATE_POLL } from '../../modules/other/types'

import Results from '../results.vue'
import Modal from '../modal.vue'

@Component({
    props: {
        onModalClose: Function
    },
	components: {
        Results, Modal
	}
})
export default class Polls extends Vue {
    @State(state => state.vm) vm: PageViewModel
    @ModuleAction(CREATE_POLL) createPoll: (payload: { poll: CrudPoll }) => Promise<Result[]>

    onModalClose: () => void

    creating: boolean = false
    isCopied: boolean = false

    newPoll: CrudPoll = new CrudPoll()
    newSelection: string = ''

    results: Result[] = []

    $modal = (this as any).VModal

	mounted() {

    }

    get canCreate(): boolean {
        return this.newPoll.name && this.newPoll.selections.length > 1 ? true : false
    }

    get maxNumberOfSelections(): boolean {
        return this.newPoll.selections.length == 10 ? true : false
    }

    get selectionPlaceholder(): string {
        return this.maxNumberOfSelections ? 'Max antal alternativ' : 'Nytt alternativ'
    }

    addToSelections() {
        if (!this.newPoll.selections.includes(this.newSelection)) {
            this.newPoll.selections.push(this.newSelection)
            this.newSelection = ''
        }
    }

    removeSelection(selection: string) {
        this.newPoll.selections = this.newPoll.selections.filter((s: string) => s !== selection)
    }

    create() {
        this.creating = true
        this.createPoll({ poll: this.newPoll })
            .then((results: Result[]) => {
                this.results = results
                this.creating = false
                this.newPoll = new CrudPoll()
                this.$modal.hide('new-poll')
            })
    }
}
</script>