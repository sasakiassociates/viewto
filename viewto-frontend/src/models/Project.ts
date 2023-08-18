import { computed } from 'mobx';
import { Model, model, modelAction, prop } from 'mobx-keystone';


@model("viewto/Project")
export class Project extends Model({
    // the name of the proejct
    name : prop<string>("some project"),
    // the id of the speckle project (stream)  
    id: prop<string>(import.meta.env.VITE_VIEWTO_TEST_PROJECT),
    // the path to the speckle model(branch) 
    model: prop<string>(import.meta.env.VITE_VIEWTO_TEST_MODEL),
    // the id to the speckle version(commit) 
    version: prop<string>(import.meta.env.VITE_VIEWTO_TEST_VERSION).withSetter(),
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

