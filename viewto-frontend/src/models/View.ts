import { Model, model, prop } from 'mobx-keystone';
import { Point } from './Point';
import { action, computed, observable } from 'mobx';
import { ViewDataCondition, ViewDataFilter } from './ViewData';


@model("viewto/View")
export class View extends Model({
    // readable name of the view  
    name: prop<string>(''),
    // generated id of view
    id: prop<string>(''),
    // referenced point the view was created
    pointId: prop<string>(''),
    // properties of the camerea details 
    // TODO: figure out what properties we need to store
    camera: prop<string>(''),
    // pixel and value modifiers 
    filter: prop<ViewDataFilter>(() => new ViewDataFilter({})),
    // the information around the focuses and obstructors
    condition: prop<ViewDataCondition>(() => new ViewDataCondition({}))
}) {

    @computed
    get point() {
        return null;
    }

    @observable
    active: boolean = false;

    @action
    setActive(active: boolean) {
        this.active = active;
    }
}


