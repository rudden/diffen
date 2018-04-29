<template>
    <ul class="list-group media-list media-list-stream">
        <li class="list-group-item p-4">
            <h4 class="mb-0">
                {{ selectedChronicleSlug ? 'editera en krönika' : 'skriv en ny krönika' }}
            </h4>
        </li>
        <li class="media list-group-item p-4" v-show="loading">
            <loader v-bind="{ background: '#699ED0' }" />
        </li>
        <div v-show="!loading">
            <li class="list-group-item media p-4" style="padding-right: 0.5rem !important;">
                <div class="row" style="width: 100%">
                    <div class="col" style="padding-right: 0">
                        <div class="form-group">
                            <input type="text" v-model="newChronicle.title" class="form-control" placeholder="titel" />
                        </div>
                        <div class="form-group">
                            <div class="custom-file" v-if="!hasSelectedHeaderFile">
                                <input type="file" class="custom-file-input" id="headerFile" accept=".png,.jpg,.jpeg" @change="handleImageAdded" />
                                <label class="custom-file-label" for="headerFile">välj en bild till headern</label>
                            </div>
                            <div class="alert alert-primary" v-else>
                                <strong>{{ headerFileName }}</strong>
                                <button type="button" class="close" aria-label="Close" v-on:click="deSelectFile">
                                    <span aria-hidden="true">&times;</span>
                                </button>
                            </div>
                        </div>
                        <div class="form-group">
                            <date-picker v-model="publishDate" :config="dpConfig" :placeholder="'publiceringsdatum (direkt om inget väljs)'" />
                        </div>
                        <div class="form-group">
                            <vue-editor v-model="content" :placeholder="'din krönika'" :editor-toolbar="customToolbar" />
                        </div>
                        <results :items="results" class="pb-3" />
                        <div class="row">
                            <div class="col">
                                <button class="btn btn-sm btn-block btn-success" v-on:click="save" :disabled="!canCreate">
                                    {{ selectedChronicleSlug ? 'spara' : 'skapa' }}
                                </button>
                            </div>
                            <div class="col">
                                <a class="btn btn-sm btn-block btn-secondary" href="/chronicle">visa lista med krönikor</a>
                            </div>
                        </div>
                    </div>
                </div>
            </li>
            <template v-if="content">
                <li class="list-group-item p-4">
                    <h4 class="mb-0">preview</h4>
                </li>
                <li class="list-group-item media p-4">
                    <div class="media-body">
                        <img :src="uploadedFileSrc" class="img-fluid mt-2" data-action="zoom">
                        <div class="preview" v-html="newChronicle.text"></div>
                    </div>
                </li>
            </template>
        </div>
    </ul>
</template>

<script lang="ts">
import Vue from 'vue'
import axios, { AxiosPromise } from 'axios'
import { Component, Watch } from 'vue-property-decorator'
import { Getter, Action, Mutation, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('other', Getter)
const ModuleAction = namespace('other', Action)
const ModuleMutation = namespace('other', Mutation)

import { PageViewModel, Result, ResultType } from '../../model/common'
import { Chronicle } from '../../model/other'
import { Chronicle as CrudChronicle } from '../../model/other/crud'

import { GET_CHRONICLE, FETCH_CHRONICLE, CREATE_CHRONICLE, UPDATE_CHRONICLE } from '../../modules/other/types'

import Results from '../results.vue'

import { VueEditor } from 'vue2-editor'
import DatePicker from 'vue-bootstrap-datetimepicker'

@Component({
    props: {
        selectedChronicleSlug: String
    },
	components: {
        Results, VueEditor, DatePicker
	}
})
export default class NewChronicle extends Vue {
    @State(state => state.vm) vm: PageViewModel
    @ModuleGetter(GET_CHRONICLE) chronicle: Chronicle
    @ModuleAction(FETCH_CHRONICLE) loadChronicle: (payload: { slug: string }) => Promise<void>
    @ModuleAction(CREATE_CHRONICLE) createChronicle: (payload: { chronicle: CrudChronicle }) => Promise<Result[]>
    @ModuleAction(UPDATE_CHRONICLE) updateChronicle: (payload: { chronicle: CrudChronicle }) => Promise<Result[]>

    selectedChronicleSlug: string

    newChronicle: CrudChronicle = new CrudChronicle()

    loading: boolean = false

    content: string = ''

    results: Result[] = []

    headerFile: FormData
    headerFileName: string = ''
    headerFileSrcPreview: string = ''

    publishDate: Date = new Date('')

    customToolbar = [
        [ { 'size': [ 'small', false, 'large', 'huge' ] } ],
        [ 'bold', 'italic', 'underline', 'strike' ],
        [ { 'align': [] } ],
        [ 'blockquote' ],
        [ { 'color': [] }, { 'background': [] } ],
        [ { 'list': 'ordered' }, { 'list': 'bullet' } ],
        [ 'link', 'video' ]
    ]

    dpConfig: any = { 
		format: 'YYYY-MM-DD', 
		useCurrent: false, 
		locale: 'sv', 
		icons: { 
			next: 'icon icon-arrow-right',
			previous: 'icon icon-arrow-left' 
        },
        widgetPositioning: {
            vertical: 'bottom',
            horizontal: 'left'
        },
        minDate: new Date(new Date().toISOString().slice(0, 10).toString() + ' 00:00')
    }

    private directoryName: string = 'chronicles'

	mounted() {
        if (this.selectedChronicleSlug) {
            this.loading = true
            this.loadChronicle({ slug: this.selectedChronicleSlug })
                .then(() => {
                    this.newChronicle = {
                        id: this.chronicle.id,
                        title: this.chronicle.title,
                        text: this.chronicle.text,
                        writtenByUserId: this.chronicle.writtenByUser.id,
                        published: this.chronicle.published
                    }
                    this.publishDate = new Date(this.chronicle.published)
                    this.content = this.chronicle.text
                    this.headerFileName = this.friendlyFileName(this.chronicle.headerFileName)
                    this.loading = false
                })
        }

        this.newChronicle.writtenByUserId = this.vm.loggedInUser.id
    }

    get canCreate() {
        return this.newChronicle.title && this.newChronicle.text ? true : false
    }

    get hasSelectedHeaderFile(): boolean {
        return this.headerFileName !== '' ? true : false
    }

    get uploadedFileSrc() {
        if (this.chronicle.id) {
            if (this.chronicle.headerFileName.includes('uploads') && this.headerFileName && this.chronicle.headerFileName.includes(this.headerFileName)) {
                return `/${this.chronicle.headerFileName}`
            }
        }
        if (this.headerFileSrcPreview)
            return this.headerFileSrcPreview
        return this.headerFileName ? `/${this.headerFileName}` : '/banner.jpg'
    }

    @Watch('content')
        onChange() {
            // modify markup generated by plugin
            this.newChronicle.text = this.content
                .replace('img', 'img class="img-fluid" ')
                .replace('ql-align-left', 'text-left')
                .replace('ql-align-right', 'text-right')
                .replace('ql-align-center', 'text-center')
                .replace('ql-size-huge', 'h1')
                .replace('ql-size-large', 'h3')
        }

    save() {
        this.loading = true
        this.newChronicle.published = this.publishDate.toString()
        if (this.newChronicle.id) {
            this.updateChronicle({ chronicle: this.newChronicle })
                .then((results: Result[]) => {
                    this.results = results
                    if (this.headerFileName !== this.friendlyFileName(this.chronicle.headerFileName)) {
                        this.uploadFile().then(() => this.loading = false)
                    } else {
                        this.loading = false
                    }
                })
        } else {
            this.createChronicle({ chronicle: this.newChronicle })
                .then((results: Result[]) => {
                    this.results = results
                    new Promise<void>((resolve, reject) => {
                        if (!this.hasSelectedHeaderFile)
                            resolve()
                        else {
                            this.uploadFile()
                                .then((result) => {
                                    this.deSelectFile()
                                    resolve()
                                })
                            
                        }
                    }).then(() => {
                        this.content = ''
                        this.newChronicle = new CrudChronicle()
                        this.loading = false
                    })
                })
        }
    }

    uploadFile(): Promise<void> {
        return new Promise<void>((resolve, reject) => {
            axios.post(`${this.vm.api}/uploads/${this.directoryName}`, this.headerFile, { headers: { 'Content-Type': 'application/json' } })
                .then((result) => {
                    let fileName: string = result.data
                    axios.post(`${this.vm.api}/chronicles/image/header/update/${fileName}`)
                        .then((res) => {
                            if (res) {
                                this.headerFileName = this.friendlyFileName(fileName)
                            }
                            resolve()
                        })
                }).catch(() => {
                    resolve()
                    this.results.push({ type: ResultType.Failure, message: 'Kunde inte ladda upp den nya header-bilden. Trolig orsak är att den är för stor. Default-bild sätts istället.' })
                })
        })
    }

    async handleImageAdded(e: any) {
        var files = e.target.files
        if (!files.length)
            return
        this.headerFile = new FormData()
        this.headerFile.append('file', files[0])
        this.headerFileName = files[0].name
        this.headerFileSrcPreview = await this.getPreviewSrc(files[0])
    }

    deSelectFile() {
        this.headerFile = new FormData()
        this.headerFileName = ''
        this.headerFileSrcPreview = ''
    }

    friendlyFileName(fileName: string) {
        let friendlyFileName = fileName.split('_____')
        return friendlyFileName[friendlyFileName.length - 1]
    }

    getPreviewSrc(files: any) {
        var reader = new FileReader()
        return new Promise<string>((resolve, reject) => {
            reader.onload = () => resolve(reader.result)
            reader.readAsDataURL(files)
        })
    }
}
</script>

<style lang="scss" scoped>
a {
    cursor: pointer;
}
</style>