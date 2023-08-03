
import { Model, Ref, computedTree, model, prop } from 'mobx-keystone';

@model("viewto/Project")
export class Project extends Model({
    // the id of the speckle project (stream)  
    id: prop<string>(),
    // the path to the speckle model(branch) 
    model: prop<string>(),
    // the id to the speckle version(commit) 
    version: prop<string>()
}) {

    @computedTree
    get score(): number {
        return 0;
    }

    @computedTree
    get views(): Ref<object>[] {
        return null;
    }

}
