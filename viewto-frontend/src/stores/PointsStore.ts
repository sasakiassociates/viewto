import { stores, Store } from '@strategies/stores';
import { computed, makeObservable, observable } from 'mobx';

import Stores from './Stores';
import { Point } from '../models/Point';


export default class PointsStore extends Store {

    constructor() {
        super();
        makeObservable(this);
    }

    @computed
    get all() {
        return (stores as Stores).scenario.points;
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

    @computed
    get sorted() {
        return [...this.all].sort(this.sortFn);
    }

    @observable
    sortFn: (a: Point, b: Point) => number = (_,__) => 0;

    
   
}
