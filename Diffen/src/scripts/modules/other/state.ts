import { Poll, Chronicle, Region } from '../../model/other'

export default class State {
    poll: Poll = new Poll()
    polls: Poll[] = []
    chronicle: Chronicle = new Chronicle()
    chronicles: Chronicle[] = []
    regions: Region[] = []
}