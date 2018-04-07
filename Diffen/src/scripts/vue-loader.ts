import Vue from 'vue'
import store from './store'

import { ViewModel } from './model/common'

export default class VueInstance {
    el: string = ''
    scriptId: string = ''
    component: any

    constructor(el: string, scriptId: string, component: any) {
        this.el = el
        this.scriptId = scriptId
        this.component = component
    }

    init(): void {
        store.state.vm = this.vm()
        
        new Vue({ 
            store,
            el: this.el,
            render: h => h(this.component)
        })
    }

    vm(): ViewModel {
        let dm: any = ''
        var el = document.getElementById(this.scriptId)
        if (el) dm = el.getAttribute('view-model')
        return JSON.parse(dm)
    }
}