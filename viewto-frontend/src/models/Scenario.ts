import { action, computed, observable } from 'mobx';
import { Model, model, prop } from 'mobx-keystone';
import { Project } from './Project';
import { View } from './View';
import { Point } from './Point';
import { FocusContext, ObstructingContext } from './Context';

@model("viewto/Scenario")
export class Scenario extends Model({
    // the information linked to the speckle project loaded
    project: prop<Project>(),
    // the list of views stored to apply the settings 
    views: prop<View[]>(() => []),
}) {

    // returns the list of points found in the views 
    @computed
    get Points(): Point[] {
        return this.views.map((x) => x.point);
    }

    // gets all the focuses from the views saved
    @computed
    get Focuses(): FocusContext[] {
        const data = [] as FocusContext[];
        this.views.map((x) => x.condition.focuses.map((child) => data.push(child)));
        return data;
    }

    // get all the obstructing conditions from the views
    @computed
    get Conditions(): ObstructingContext[] {
        const data = [] as ObstructingContext[];
        this.views.map((x) => x.condition.obstructors.map((child) => data.push(child)));
        return data
    }

    @observable
    data: any;

    @action
    setData(data: any) {
        this.data = data;
    }
}