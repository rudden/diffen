<template>
    <ul class="list-group media-list media-list-stream">
        <li class="list-group-item p-4">
            <h4 class="mb-0">
                {{ selectedChronicleSlug ? 'Editera en krönika' : 'Skriv en ny krönika' }}
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
                            <input type="text" v-model="newChronicle.title" class="form-control" placeholder="Titel" />
                        </div>
                        <div class="form-group">
                            <div class="custom-file" v-if="!hasSelectedHeaderFile">
                                <input type="file" class="custom-file-input" id="headerFile" accept=".png,.jpg,.jpeg" @change="handleImageAdded" />
                                <label class="custom-file-label" for="headerFile">Välj en bild till headern</label>
                            </div>
                            <div class="alert alert-primary" v-else>
                                <strong>{{ headerFileName }}</strong>
                                <button type="button" class="close" aria-label="Close" v-on:click="deSelectFile">
                                    <span aria-hidden="true">&times;</span>
                                </button>
                            </div>
                        </div>
                        <div class="form-group">
                            <v-multiselect v-model="selectedCategories"
                                :options="allCategories"
                                :multiple="true"
                                :close-on-select="false"
                                :clear-on-select="false"
                                :hide-selected="true"
                                :preserve-search="true"
                                placeholder="Välj minst en kategori"
                                label="name"
                                track-by="id"
                                :taggable="true"
                                @tag="addCategory">
                                <template slot="tag" slot-scope="props">
                                    <span class="custom__tag">
                                        <span class="badge badge-primary mr-2 p-1">
                                            <span>
                                                <span class="icon icon-tag mr-1"></span>
                                                {{ props.option.name }}
                                            </span>
                                            <span class="custom__remove" @click="props.remove(props.option)">&times;</span>
                                        </span>
                                    </span>
                                </template>
                            </v-multiselect>
                        </div>
                        <div class="form-group">
                            <date-picker v-model="publishDate" :config="dpConfig" :placeholder="'Publiceringsdatum (direkt om inget väljs)'" />
                        </div>
                        <div class="form-group">
                            <vue-editor v-model="content" :placeholder="'Din krönika'" :editor-toolbar="customToolbar" />
                        </div>
                        <results :items="results" class="pb-3" />
                        <div class="row">
                            <div class="col">
                                <button class="btn btn-sm btn-block btn-success" v-on:click="save" :disabled="!canCreate">
                                    {{ selectedChronicleSlug ? 'Spara' : 'Skapa' }}
                                </button>
                            </div>
                            <div class="col">
                                <a class="btn btn-sm btn-block btn-secondary" href="/kronika">Visa lista med krönikor</a>
                            </div>
                        </div>
                    </div>
                </div>
            </li>
            <template v-if="content">
                <li class="list-group-item p-4">
                    <h4 class="mb-0">Förhandsgranskning</h4>
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
import { Chronicle, ChronicleCategory } from '../../model/other'
import { Chronicle as CrudChronicle } from '../../model/other/crud'

import { GET_CHRONICLE, GET_CHRONICLE_CATEGORIES, FETCH_CHRONICLE_CATEGORIES, FETCH_CHRONICLE, CREATE_CHRONICLE, UPDATE_CHRONICLE } from '../../modules/other/types'

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
    @ModuleGetter(GET_CHRONICLE_CATEGORIES) categories: ChronicleCategory[]
    @ModuleAction(FETCH_CHRONICLE) loadChronicle: (payload: { slug: string }) => Promise<void>
    @ModuleAction(FETCH_CHRONICLE_CATEGORIES) loadCategories: () => Promise<void>
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

    newCategories: ChronicleCategory[] = []
    selectedCategories: ChronicleCategory[] = []

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
        }
    }

    private directoryName: string = 'chronicles'

	mounted() {
        this.loadCategories()
        if (this.selectedChronicleSlug) {
            this.loading = true
            this.loadChronicle({ slug: this.selectedChronicleSlug })
                .then(() => {
                    this.newChronicle = {
                        id: this.chronicle.id,
                        title: this.chronicle.title,
                        text: this.chronicle.text,
                        writtenByUserId: this.chronicle.writtenByUser.id,
                        categoryIds: this.chronicle.categories.map((c: ChronicleCategory) => c.id),
                        published: this.chronicle.published
                    }
                    this.selectedCategories = this.chronicle.categories
                    this.publishDate = new Date(this.chronicle.published)
                    this.content = this.chronicle.text
                    this.headerFileName = this.friendlyFileName(this.chronicle.headerFileName)
                    this.loading = false
                })
        }

        this.newChronicle.writtenByUserId = this.vm.loggedInUser.id
    }

    get canCreate() {
        return this.newChronicle.title && this.newChronicle.text && this.selectedCategories.length > 0 ? true : false
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

    get allCategories() {
        return this.categories.concat(this.newCategories)
    }

    @Watch('content')
        onChange() {
            // modify markup generated by plugin
            this.newChronicle.text = this.content
                .replace('img', 'img class="img-fluid" data-action="zoom" ')
                .replace('ql-align-left', 'text-left')
                .replace('ql-align-right', 'text-right')
                .replace('ql-align-center', 'text-center')
                .replace('ql-size-huge', 'h1')
                .replace('ql-size-large', 'h3')
        }

    save() {
        this.loading = true
        this.newChronicle.published = (this as any).$helpers.getDateAsString(this.publishDate)
        this.newChronicle.newCategoryNames = this.newCategories.length > 0 ? this.newCategories.map((c: ChronicleCategory) => c.name) : undefined
        this.newChronicle.categoryIds = this.selectedCategories.filter((c: ChronicleCategory) => c.id > 0).map((c: ChronicleCategory) => c.id)
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
                        this.newCategories = []
                        this.selectedCategories = []
                        this.newChronicle = new CrudChronicle()
                        this.loading = false
                    })
                })
        }
    }

    uploadFile(chronicleId?: number): Promise<void> {
        return new Promise<void>((resolve, reject) => {
            axios.post(`${this.vm.api}/uploads/${this.directoryName}`, this.headerFile, { headers: { 'Content-Type': 'application/json' } })
                .then((result) => {
                    let fileName: string = result.data
                    axios.post(`${this.vm.api}/chronicles/image/header/update/${fileName}?chronicleId=${chronicleId}`)
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

    addCategory(newCategory: string) {
        const category = {
            id: 0,
            name: newCategory
        }
        this.newCategories.push(category)
        this.selectedCategories.push(category)
    }
}
</script>

<style lang="scss" scoped>
a {
    cursor: pointer;
}
</style>
