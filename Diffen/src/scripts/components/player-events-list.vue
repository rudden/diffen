<template>
    <div>
        <li><strong>{{ header }}</strong></li>
        <li class="ellipsis" v-for="item in items" :key="`${keyBase}-${item.player.id}`">
            <modal v-bind="{ attributes: { name: `${keyBase}-${item.player.id}` }, header: `${item.player.fullName}s ${typeName}`, button: { badge: 'badge-primary float-right', text: item.total } }" v-if="!isSmall">
                <template slot="body">
                    <div v-for="event in item.events">
                        <v-popover trigger="click" :placement="'left'" :auto-hide="true" class="float-right" v-if="event.amount > 0">
                            <span class="badge badge-primary" style="cursor: pointer">{{ event.amount }}</span>
                            <template slot="popover">
                                <ul class="list-unstyled list-spaced mb-0">
                                    <li v-for="date in event.dates">
                                        {{ date }}
                                    </li>
                                </ul>
                            </template>
                        </v-popover>
                        <span class="badge badge-danger float-right" v-else>{{ event.amount }}</span>
                        <span class="mr-2">{{ getGameType(event.type) }}</span>
                    </div>
                </template>
            </modal>
            <template v-else>
                <v-popover trigger="click" :placement="'left'" :auto-hide="true" class="float-right">
                    <span class="badge badge-primary" style="cursor: pointer">{{ item.total }}</span>
                    <template slot="popover">
                        <ul class="list-unstyled list-spaced mb-0">
                            <li v-for="date in dates(item.events)">
                                {{ date }}
                            </li>
                        </ul>
                    </template>
                </v-popover>
            </template>
            {{ item.player.fullName }}
        </li>
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import { Getter, Action, Mutation, State, namespace } from 'vuex-class'

import { PageViewModel } from '../model/common'
import { GameType } from '../model/squad'

interface PlayerItem {
    id: number
    fullName: string
}

interface IPlayerEvent {
    type: GameType
    dates: string[]
    amount: number
}

interface ListItem {
    player: PlayerItem
    events: IPlayerEvent[]
    total: number
}

import Modal from './modal.vue'

@Component({
    props: {
        header: String,
        items: Array,
        isSmall: {
            type: Boolean,
            default: false
        },
        typeName: String
    },
    components: {
        Modal
    }
})
export default class PlayerEventsList extends Vue {
    @State(state => state.vm) vm: PageViewModel

    header: string
    items: ListItem[]
    isSmall: boolean
    typeName: string

    $modal = (this as any).VModal

    get keyBase() {
        return this.header.toLowerCase().replace(' ', '')
    }

    getGameType(type: GameType): string {
        switch (type) {
            case GameType.Cup:
                return 'Cupen'
            case GameType.League:
                return 'Allsvenskan'
            case GameType.EuropaLeague:
                return 'Europa League'
            case GameType.Training:
                return 'Träningsmatch'
            default:
                return ''
        }
    }

    dates(events: IPlayerEvent[]): string[] {
        return events.filter((e: IPlayerEvent) => e.amount !== 0).map((e: IPlayerEvent) => e.dates)[0]
    }

}
</script>
