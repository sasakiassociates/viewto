import { Model, computedTree, model, prop } from 'mobx-keystone';
import { Project } from './Project';
import { View } from './View';
import { Point } from './Point';
import { FocusContext, ConditionContext } from './Context';

@model("viewto/View")
class Scenario extends Model({
    // the information linked to the speckle project loaded
    project: prop<Project>(),
    // the list of views stored to apply the settings 
    views: prop<View[]>(),
}) {

    // returns the list of points found in the views 
    @computedTree
    get Points(): Point[] {
        return this.views.map((x) => x.point);
    }

    // gets all the focuses from the views saved
    @computedTree
    get Focuses(): FocusContext[] {
        const data = [] as FocusContext[];
        this.views.map((x) => x.focuses.map((child) => data.push(child)));
        return data;
    }

    // get all the obstructing conditions from the views
    @computedTree
    get Conditions(): ConditionContext[] {
        return this.views.map((x) => x.condition);
    }

    // volatile 
    data: null;

}


export default Scenario;