import { User } from '../profile'

export class ViewModel {
	api: string
	page: string
	postId: number
	pageNumber: number
	selectedUserId: string
	loggedInUser: User = new User()
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