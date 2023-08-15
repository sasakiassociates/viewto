import { stores, Store } from '@strategies/stores';
import { action, computed, makeObservable, observable } from 'mobx';

import Stores from './Stores';
// import { Point } from '../models/Point';


export default class PointsStore extends Store {

    constructor() {
        super();
        makeObservable(this);
    }

    @computed
    get all() {
        return (stores as Stores).scenario.study.points;
    }

    @computed
    get filtered() {
        return this.all;
    }

    @computed
    get goodScores() {
        return this.all;
    }

    @computed
    get badScores() {
        return this.all;
    }

    // @computed
    // get sorted() {
    //     return [...this.all].sort((a, b) => this.sortKey(a) >= this.sortKey(b) ? this.sortDirection : -this.sortDirection);
    // }

    @observable
    sortDirection: number = 1;

    @action
    setSortDirection(direction: number) {
        this.sortDirection = direction > 0 ? 1 : -1;
    }

    // @observable
    // sortKey: (p: Point) => number|string = _ => 0;

    // @action
    // setSortKey(fn: (p: Point) => number|string) {
    //     this.sortKey= fn;
    // }

}
