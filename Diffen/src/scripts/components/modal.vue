<template>
    <div class="wrap">
        <slot name="btn"></slot>
        <div class="modal fade" :id="id" tabindex="-1" role="dialog" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title">{{ header }}</h4>
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true" v-on:click="close">&times;</button>
                    </div>
                    <div class="modal-body">
                        <slot name="body"></slot>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'

@Component({
    props: {
        id: String,
        header: String,
        closeAction: Function
    }
})
export default class Modal extends Vue {
    id: string
    header: string
    closeAction: Function

    close() {
        if (this.closeAction) {
            this.closeAction()
        }
    }
}
</script>
