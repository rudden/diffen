<template>
    <div class="results" v-if="results.length > 0">
        <div class="alert alert-success alert-dismissible fade show" v-if="successes.length > 0">
            <ul>
                <li v-for="success in successes">
                    {{ success.message }}
                </li>
            </ul>
            <button type="button" class="close" data-dismiss="alert" aria-label="Close" v-on:click="dismiss(successType)">
                <span aria-hidden="true">&times;</span>
            </button>
        </div>
        <div class="alert alert-danger alert-dismissible fade show" v-if="failures.length > 0">
            <ul>
                <li v-for="failure in failures">
                    {{ failure.message }}
                </li>
            </ul>
            <button type="button" class="close" data-dismiss="alert" aria-label="Close" v-on:click="dismiss(failureType)">
                <span aria-hidden="true">&times;</span>
            </button>
        </div>
    </div>
</template> 

<script lang="ts">
import Vue from 'vue'
import { Component, Watch } from 'vue-property-decorator'

import { Result, ResultType } from '../model/common'

@Component({
    props: {
        items: Array,
        dismiss: Function
    }
})
export default class Results extends Vue {
    items: Result[]
    dismiss: (type: ResultType) => void

    results: Result[] = []

    created() {
        this.results = this.items
    }

    @Watch('items')
        change() {
            this.results = this.items
        }

    get successType(): ResultType { return ResultType.Success }
    get failureType(): ResultType { return ResultType.Failure }

    get successes(): Result[] {
        return this.results.filter((r: Result) => r.message != '' && r.type == ResultType.Success)
    }

    get failures(): Result[] {
        return this.results.filter((r: Result) => r.message != '' && r.type == ResultType.Failure)
    }
}
</script>

<style lang="scss" scoped>
.results {
    .alert:last-child {
        margin-bottom: 0;
    }
    ul {
        padding-left: 0;
        margin-bottom: 0;
        li {
            list-style: none;
        }
    }
}
</style>
