import Vue from 'vue'
import store from './store'

import { ViewModel } from './model/common'
import { Component as VueComponent } from 'vue/types/options'

export default class VueInstance {
    el: string = ''
    component: VueComponent

    constructor(el: string, component: VueComponent) {
        this.el = el
        this.component = component
    }

    init(): void {
        store.state.vm = this.vm()
        
        new Vue({ 
            store,
            el: `#${this.el}`,
            render: h => h(this.component)
        })
    }

    vm(): ViewModel {
        let dm: any = ''
        var el = document.getElementById(this.el)
        if (el) dm = el.getAttribute('view-model')
        return JSON.parse(dm)
    }
}