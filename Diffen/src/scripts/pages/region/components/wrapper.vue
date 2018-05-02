<template>
	<div>
		<navbar />
		<div class="container pt-4 pb-5">
			<div class="col">
				<ul class="list-group media-list media-list-stream" id="top">
					<li class="list-group-item p-4">
						<modal v-bind="modalAttributes.newRegion" v-if="loggedInUserIsScissorOrAdmin">
							<template slot="body">
								<template v-if="creating">
									<loader v-bind="{ background: '#699ED0' }" />
								</template>
								<template v-else>
									<div class="row">
										<div class="col">
											<div class="form-group">
												<google-autocomplete @place_changed="selectPlace" class="form-control form-control-sm" placeholder="sök på en plats"></google-autocomplete>
												<template v-if="newRegion.name">
													<google-map :center="newRegionPosition" :zoom="zoom" style="width: 100%; height: 350px; margin-top: 1rem">
														<google-marker :position="newRegionPosition" :clickable="true" :draggable="true"></google-marker>
													</google-map>
												</template>
											</div>
										</div>
									</div>
									<results :items="results" class="pb-3" />
									<div class="row">
										<div class="col">
											<button class="btn btn-success btn-sm btn-block" v-on:click="create" :disabled="!canCreate">skapa</button>
										</div>
									</div>
								</template>
							</template>
						</modal>
						<h4 class="mb-0">Områden</h4>
					</li>
					<li class="media list-group-item p-0">
						<google-map :center="center" :zoom="zoom" style="width: 100%; height: 500px">
							<google-marker v-for="m in markers" :key="m.position.lat" :position="m.position" :clickable="true" :draggable="true" @click="center = m.position"></google-marker>
						</google-map>
					</li>
					<li class="media list-group-item p-4">
						<span class="icon icon-location-pin text-muted mr-2"></span>
						<div class="media-body">
							<div class="form-group float-right mb-0">
								<input type="text" class="form-control form-control-sm" v-model="regionSearch" placeholder="Sök">
							</div>
							<div class="media-heading">
								<a href="#top" @click="showAll">Alla områden</a>
							</div>
						</div>
					</li>
					<li class="media list-group-item p-4" v-show="loading">
						<loader v-bind="{ background: '#699ED0' }" />
					</li>
					<div v-show="!loading">
						<template v-if="filteredRegions.length > 0">
							<li class="list-group-item media p-4" v-for="region in filteredRegions" :key="region.id">
								<span class="icon icon-location-pin text-muted mr-2"></span>
								<div class="media-body">
									<span class="text-muted float-right">
										<template v-if="region.users.length > 0">
											<modal v-bind="{ attributes: { name: `${region.name}-users` }, header: `användare i ${region.name}`, button: { badge: 'badge-primary', text: `${region.users.length} användare` } }">
												<template slot="body">
													<ul class="list-unstyled">
														<li v-for="user in region.users" :key="user.id">
															<a :href="`/profile/${user.id}`">{{ user.nickName }}</a>
														</li>
													</ul>
												</template>
											</modal>
										</template>
										<template v-else>
											<span class="badge badge-primary">{{ region.users.length }} användare</span>
										</template>
									</span>
									<div class="media-heading">
										<a href="#top" @click="setPlace(region)">{{ region.name }}</a>
									</div>
								</div>
							</li>
						</template>
						<template v-else>
							<li class="list-group-item media p-4">
								<div class="col p-0">
									<div class="alert alert-warning mb-0">Hittade inga områden</div>
								</div>
							</li>
						</template>
					</div>
				</ul>
			</div>
		</div>
	</div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component, } from 'vue-property-decorator'
import { Getter, Action, State, namespace } from 'vuex-class'

const ModuleGetter = namespace('other', Getter)
const ModuleAction = namespace('other', Action)

import { GET_REGIONS, FETCH_REGIONS, CREATE_REGION } from '../../../modules/other/types'

import { Region } from '../../../model/other'
import { Region as CrudRegion } from '../../../model/other/crud'
import { RegionViewModel, Result } from '../../../model/common'

import Modal from '../../../components/modal.vue'
import Results from '../../../components/results.vue'

import * as VueGoogleMaps from 'vue2-google-maps'
const GoogleMap = require('vue2-google-maps').Map
const GoogleMarker = require('vue2-google-maps').Marker
const GoogleAutocomplete = require('vue2-google-maps').Autocomplete

@Component({
	components: {
		Modal, Results,
		VueGoogleMaps, GoogleMap, GoogleMarker, GoogleAutocomplete
	}
})
export default class Wrapper extends Vue {
	@State(state => state.vm) vm: RegionViewModel
	@ModuleGetter(GET_REGIONS) regions: Region[]
	@ModuleAction(FETCH_REGIONS) loadRegions: () => Promise<void>
	@ModuleAction(CREATE_REGION) createRegion: (payload: { region: CrudRegion }) => Promise<Result[]>

	loading: boolean = true
	creating: boolean = false

	zoom: number = 15
	center: any = {}
	markers: any = []

	newRegion: CrudRegion = new CrudRegion()
	newRegionPosition: any = {}

	modalAttributes: any = {
        newRegion: {
            attributes: {
                name: 'new-region'
            },
            header: 'nytt område',
            button: {
                classes: 'btn btn-sm btn-primary float-right',
                text: 'skapa nytt område'
			},
			onClose: this.resetCrudRegion
        }
	}
	
	results: Result[] = []

	regionSearch: string = ''

	created() {
		// register here instead of store.ts
		// because navbar component is loaded with same store (error when maps component is initiated twice on same page)
		Vue.use(VueGoogleMaps, {
			load: {
				key: this.vm.googleMapsApiKey,
				libraries: "places" // necessary for places input
			},
			installComponents: true
		}, this.$store)
	}

    mounted() {
		this.loadRegions().then(() => this.loading = false)

		let position = {
			lat: 59.345446,
			lng: 18.079061
		}
		this.markers.push({ position })
		this.center = position
	}

	get loggedInUserIsScissorOrAdmin() {
		return this.vm.loggedInUser.inRoles.some(role => role == 'Admin' || role == 'Scissor')
	}
	get canCreate() {
		return this.newRegion.name && this.newRegion.longitud > 0 && this.newRegion.latitud > 0 ? true : false
	}
	get filteredRegions() {
		return this.regions.filter((r: Region) => {
			return r.name.toLowerCase().includes(this.regionSearch.toLowerCase())
		})
	}

	setPlace(region: Region) {
		this.zoom = 12
		this.markers = []
		let position = {
			lat: region.latitud,
			lng: region.longitud
		}
		this.markers.push({ position })
		this.center = position
	}

	selectPlace(place: any) {
		if (!place.name) {
			this.resetCrudRegion()
		} else {
			this.newRegion = {
				name: place.name,
				latitud: place.geometry.location.lat(),
				longitud: place.geometry.location.lng()
			}
			this.newRegionPosition = {
				lat: this.newRegion.latitud,
				lng: this.newRegion.longitud
			}
		}
	}

	showAll() {
		this.zoom = 9
		this.markers = []
		this.regions.forEach((r: Region) => {
			this.markers.push({
				position: {
					lat: r.latitud,
					lng: r.longitud
				}
			})
		})
	}

	resetCrudRegion() {
		this.newRegion = new CrudRegion()
	}

	create() {
		this.creating = true
		this.createRegion({ region: this.newRegion })
			.then((results: Result[]) => {
				this.results = results
				this.creating = false
				this.loadRegions()
			})
	}
}
</script>

<style lang="scss" scoped>

</style>
