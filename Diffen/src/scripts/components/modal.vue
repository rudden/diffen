<template>
    <div class="wrap">
        <a :class="button.classes" v-on:click="show">
            <template v-if="button.icon">
                <span :class="button.icon" v-tooltip="button.text"></span>
            </template>
            <template v-else-if="button.badge">
                <span class="badge" :class="button.badge">{{ button.text }}</span>
            </template>
            <template v-else>
                {{ button.text }}
            </template>
        </a>
        <v-modal v-bind="attributes" @before-open="open" @before-close="close">
            <div class="modal-header" v-if="header">
                <h4 class="modal-title">{{ header }}</h4>
                <button type="button" class="close" v-on:click="closeOnIconClick">
                    <span>&times;</span>
                </button>
            </div>
            <div class="modal-body">
                <slot name="body"></slot>
            </div>
            <div class="modal-footer" v-if="hasFooterSlot">
                <slot name="footer"></slot>
            </div>
        </v-modal>
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'

@Component({
    props: {
        attributes: Object,
        header: String,
        button: Object,
        onOpen: Function,
        onClose: Function
    }
})
export default class Modal extends Vue {
    attributes: any
    header: string
    button: any
    onOpen: () => void
    onClose: () => void

    $modal = (this as any).VModal

    created() {
        if (!this.button.classes) {
            this.button.classes = ''
        }
        if (!this.hasButtonClass('btn') && !this.hasButtonClass('on-click') && !this.button.icon) {
            this.button.classes += ' on-click'
        }
        this.attributes.maxWidth = 900
        this.attributes.height = 'auto'
        this.attributes.adaptive = true
    }

        
    get hasFooterSlot() {
        return !!this.$slots.footer
    }

    close() {
        if (this.onClose) {
            this.onClose()
        }
    }
    closeOnIconClick() {
        if (this.onClose) {
            this.onClose()
        }
        this.$modal.hide(this.attributes.name)
    }

    open() {
        if (this.onOpen) {
            this.onOpen()
        }
    }

    show() {
        this.$modal.show(this.attributes.name)
    }

    hasButtonClass(className: string) {
        return this.button.classes.split(' ').includes(className) ? true : false
    }
}
</script>


<style lang="scss" scoped>
a {
    cursor: pointer;
}
a.small {
    font-size: 80%;
    font-weight: 400;
}
a.on-click {
	color: #3097D1;
    text-decoration: none;
    background-color: transparent;
	&:hover {
		color: #3097D1;
        cursor: pointer;
		text-decoration: underline;
	}
}
.btn {
    color: white !important;
}
</style>