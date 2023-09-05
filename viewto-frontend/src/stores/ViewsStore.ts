import { stores, Store } from '@strategies/stores';
import { action, computed, makeObservable, observable } from 'mobx';

import Stores from './Stores';
import { PointView } from '../models/View';


export default class ViewsStore extends Store {

    private readonly virtual = new PointView({});

    constructor() {
        super();
        makeObservable(this);
    }

    @computed
    get all() {
        return (stores as Stores).scenario.views;
    }

    @computed
    get active(): PointView {
        return this.all.filter(v => v.active)[0] || this.virtual;
    }

    // @observable
    // cloud?: ResultCloud;

    // @action
    // setCloud(value: ResultCloud) {
    //     this.cloud = value;
    // }



    // /**
    //  * Gets a single result value that match the names of conditions
    //  *
    //  * @param focus - the name of the focus context to look for
    //  * @param obstructor - the name of the obstructor to look for
    //  * @returns returns the raw value of the result data
    //  *
    //  */
    // public setCondition(focus: string, obstructor: string) {
    //     let values: number[] = [];

    //     if (!this.cloud) {
    //         console.warn('No cloud is set to this explorer');
    //         return values;
    //     }

    //     console.log(`View Condition Input\nFocus:${focus}\nObstructor:${obstructor}`);

    //     for (let i = 0; i < this.cloud.results.length; i++) {
    //         const condition = this.cloud.results[i].condition;

    //         if (condition.focusId !== focus || condition.obstructorId !== obstructor) continue

    //         values = [...this.cloud.results[i].values];
    //         break;
    //     }

    //     this.values = remapSols(values, this.modifiers.solRange.min, this.modifiers.solRange.max);
    //     this.colors = this.values?.map(x => this.modifiers.gradient.Color(x));
    // }

    // @computed
    // get getActiveResultCloud() {

    //     const active = this..filter(ref => ref.active);

    //     if (!active || active.length === 0) {
    //         console.warn('No results are active');
    //         return;
    //     }

    //     if (active.length > 1) {
    //         console.log(`To many active results(expected 1): ${active.length}\nWill return first item`);
    //     }

    //     return active[0];
    // }

    @observable
    hasActiveCloud: boolean = false;

    @action
    setHasActiveCloud(value: boolean) {
        this.hasActiveCloud = value;
    }



    /*
    @observable
    viewFocus: string[] = [];

    @action
    setViewFocus(viewFocus: string[]) {
        this.viewFocus = viewFocus;
    }

    @observable
    viewCondition: string = '';

    @action
    setViewCondition(viewCondition: string) {
        this.viewCondition = viewCondition;
    }

    @observable
    totalPoints: number = 0;

    @action
    setTotalPoints(totalPoints: number) {
        this.totalPoints = totalPoints;
    }

    @observable
    visiblePoints: number = 0;

    @action
    setVisiblePoints(visiblePoints: number) {
        this.visiblePoints = visiblePoints;
    }

    @observable
    goodViewPoints: number = 0;

    @action
    setGoodViewPoints(goodViewPoints: number) {
        this.goodViewPoints = goodViewPoints;
    }

    @observable
    noViewPoints: number = 0;

    @action
    setNoViewPoints(noViewPoints: number) {
        this.noViewPoints = noViewPoints;
    }
    */

}
