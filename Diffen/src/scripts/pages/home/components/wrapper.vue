<template>
	<div>
		<navbar />
		<div id="indicators" class="carousel slide" data-ride="carousel">
			<div class="carousel-inner">
				<div class="carousel-item active">
					<img class="d-block w-100" src="stadion.png" alt="First slide">
				</div>
				<div class="carousel-item">
					<img class="d-block w-100" src="tifo.png" alt="Second slide">
				</div>
				<div class="carousel-item">
					<img class="d-block w-100" src="tele2.png" alt="Third slide">
				</div>
			</div>
			<a class="carousel-control-prev" href="#indicators" role="button" data-slide="prev">
				<span class="carousel-control-prev-icon" aria-hidden="true"></span>
				<span class="sr-only">Previous</span>
			</a>
			<a class="carousel-control-next" href="#indicators" role="button" data-slide="next">
				<span class="carousel-control-next-icon" aria-hidden="true"></span>
				<span class="sr-only">Next</span>
			</a>
		</div>
		<div class="container" style="margin-top: 40px">
			<div class="row">
				<div class="col">
					<blockquote class="blockquote text-center">
						<p class="mb-0">
							Den fana vajar högt i skyn <br />
							I gult, rött, blått en sån härlig syn <br />
							En ström av folk från gator, torg <br />
							På väg till stadion våran borg <br />
							Hör rösterna som sjunger klart <br />
							Till Djurgårns ära så underbart <br />
							Ser spelarna gå in på plan <br />
							Stockholms stolthet bäst i stan <br />
							Å djurgårds hjärtan klappar hårt hos gamma å hos ung <br />
							Vi hyllar laget som e vårt så ställ er upp å sjung
							<br /><br />
							För hjältarna i blåa ränder <br />
							där ni går går vi bredvid <br />
							vi älskar er vad som än händer <br />
							svär er trohet i evig tid x2 <br />
						</p>
						<footer class="blockquote-footer">Sektion b</footer>
					</blockquote>
				</div>
			</div>
		</div>
	</div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'

@Component({})
export default class Wrapper extends Vue {}
</script>
