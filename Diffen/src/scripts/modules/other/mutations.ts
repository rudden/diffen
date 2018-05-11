import State from './state'
import { MutationTree } from 'vuex'

import { Poll, Chronicle, PollSelection, Region, ChronicleCategory } from '../../model/other'
import { PollVote as CrudVote } from '../../model/other/crud'

import { SET_POLL, SET_POLLS, SET_CHRONICLES, SET_CHRONICLE, SET_VOTES_ON_POLL, SET_REGIONS, SET_CHRONICLE_CATEGORIES } from './types'

export const Mutations: MutationTree<State> = {
    [SET_POLL]: (state: State, poll: Poll) => { state.poll = poll },
    [SET_POLLS]: (state: State, polls: Poll[]) => { state.polls = polls },
    [SET_CHRONICLE]: (state: State, chronicle: Chronicle) => { state.chronicle = chronicle },
    [SET_CHRONICLES]: (state: State, chronicles: Chronicle[]) => { state.chronicles = chronicles },
    [SET_CHRONICLE_CATEGORIES]: (state: State, categories: ChronicleCategory[]) => { state.chronicleCategories = categories },
    [SET_VOTES_ON_POLL]: (state: State, payload: { vote: CrudVote, pollId: number, byUserNickName: string }) => {
        if (state.polls.length > 0) {
            state.polls.filter((p: Poll) => p.id == payload.pollId)[0]
                .selections.filter((s: PollSelection) => s.id == payload.vote.pollSelectionId)[0]
                    .votes.push({
                        id: payload.vote.votedByUserId,
                        nickName: payload.byUserNickName
                    })
        } else {
            state.poll.selections.filter((s: PollSelection) => s.id == payload.vote.pollSelectionId)[0]
                .votes.push({
                    id: payload.vote.votedByUserId,
                    nickName: payload.byUserNickName
                })
        }
    },
    [SET_REGIONS]: (state: State, regions: Region[]) => { state.regions = regions }
}

export default Mutations