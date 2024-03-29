import { Model, model, prop } from 'mobx-keystone';

@model("viewto/Project")
export class Project extends Model({
    // the id of the speckle project (stream)  
    id: prop<string>("a823053e07"),
    // the path to the speckle model(branch) 
    model: prop<string>("main"),
    // the id to the speckle version(commit) 
    version: prop<string>("6d762c3c7a")
}) { }

