import { Model, model, prop } from 'mobx-keystone';
import { action, computed, observable } from 'mobx';
import { stores } from '@strategies/stores';
import Stores from '../stores/Stores';


@model("viewto/View")
export class PointView extends Model({
    // readable name of the view  
    name: prop<string>(''),
    // generated id of view
    id: prop<string>(''),
    // referenced point the view was created
    pointId: prop<string>(''),
    // properties of the camerea details 
    // TODO: figure out what properties we need to store
    camera: prop<string>(''),
    // // pixel and value modifiers 
    // filter: prop<ViewDataFilter>(() => new ViewDataFilter({})),
    focusIds: prop<string[]>(() => []).withSetter(),
    obstructorIds: prop<string[]>(() => []).withSetter(),
}) {

    @computed
    get focuses() {
        return this.focusIds.map(id => (stores as Stores).focuses.byId[id]);
    }

    @computed
    get obstructors() {
        return this.obstructorIds.map(id => (stores as Stores).obstructors.byId[id]);
    }

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


