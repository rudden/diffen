import { Poll, Chronicle, Region, ChronicleCategory } from '../../model/other'

export default class State {
    poll: Poll = new Poll()
    polls: Poll[] = []
    chronicle: Chronicle = new Chronicle()
    chronicles: Chronicle[] = []
    chronicleCategories: ChronicleCategory[] = []
    regions: Region[] = []
}