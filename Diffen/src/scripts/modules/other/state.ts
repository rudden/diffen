import { Poll, Chronicle } from '../../model/other'

export default class State {
    polls: Poll[] = []
    chronicle: Chronicle = new Chronicle()
    chronicles: Chronicle[] = []
}