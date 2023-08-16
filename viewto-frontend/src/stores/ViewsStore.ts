import { stores, Store } from '@strategies/stores';
import { action, computed, makeObservable, observable } from 'mobx';

import Stores from './Stores';
import { View } from '../models/View';


export default class ViewsStore extends Store {

    constructor() {
        super();
        makeObservable(this);
    }

    @computed
    get all() {
        return (stores as Stores).scenario.views;
    }

    @computed
    get active(): View {
        return this.all.filter(v => v.active)[0];
    }
    
    virtual = new View({});

    @observable
    viewFocus: string[] = [];

    @action
    setViewFocus(viewFocus: string[]) {
        this.viewFocus = viewFocus;
    }

    @observable
    viewCondition: string;

    @action
    setViewCondition(viewCondition: string) {
        this.viewCondition = viewCondition;
    }

    @observable
    totalPoints: number;

    @action
    setTotalPoints(totalPoints: number) {
        this.totalPoints = totalPoints;
    }

    @observable
    visiblePoints: number;

    @action
    setVisiblePoints(visiblePoints: number) {
        this.visiblePoints = visiblePoints;
    }

    @observable
    goodViewPoints: number;

    @action
    setGoodViewPoints(goodViewPoints: number) {
        this.goodViewPoints = goodViewPoints;
    }

    @observable
    noViewPoints: number;

    @action
    setNoViewPoints(noViewPoints: number) {
        this.noViewPoints = noViewPoints;
    }
   
}
