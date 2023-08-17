import { computed } from 'mobx';
import { Model, model, modelAction, prop } from 'mobx-keystone';


@model("viewto/Project")
export class Project extends Model({
    // the name of the proejct
    name : prop<string>("some project"),
    // the id of the speckle project (stream)  
    id: prop<string>("a823053e07"),
    // the path to the speckle model(branch) 
    model: prop<string>("main"),
    // the id to the speckle version(commit) 
    version: prop<string>("6d762c3c7a").withSetter(),
}) { 

    @computed
    get key() {
        return `${this.id}-${this.model}-${this.version}`;
    }

    @computed
    get complete() {
        return this.id && this.model && this.version;
    }

    @modelAction
    setId(id: string) {
        this.id = id;

        this.setModel('');
    }

    @modelAction
    setModel(model: string) {
        this.model = model;

        this.setVersion('');
    }
}

