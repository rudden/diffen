import { User } from '../profile'
import { Component as VueComponent } from 'vue/types/options'

export class PageViewModel {
	api: string
	page: string
	loggedInUser: User = new User()
}

export class ForumViewModel extends PageViewModel {
	selectedPostId: number
	selectedPageNumber: number
	fullConversationMode: boolean
	selectedThreadName: string
}

export class ProfileViewModel extends PageViewModel {
	selectedUserId: string
}

export class ChronicleViewModel extends PageViewModel {
	inCreate: boolean
	selectedChronicleSlug: string
}

export class PollViewModel extends PageViewModel {
	selectedPollSlug: string
}

export class RegionViewModel extends PageViewModel {
	googleMapsApiKey: string
}

export class KeyValuePair {
    key: any
    value: any
}

export class Paging<T> {
	data: T[] = []
	currentPage: number
	numberOfPages: number
	total: number
}

export class Result {
	type: ResultType
	message: string
}

export enum ResultType {
	Failure, Success
}

export class IdAndNickNameUser {
	id: string
	nickName: string
}

export class NavItem {
	id: number
	text?: string
	component: VueComponent
	attributes?: Object
	available?: boolean
	active: boolean
}