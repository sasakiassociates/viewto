
import { Model, Ref, computedTree, model, prop } from 'mobx-keystone';

@model("viewto/Point")
export class Point extends Model({
    // not entirely sure this is needed since the points don't have an id
    // but maybe this could be useful if we want to extract the points somehow
    sasakiId: prop<string>(),
    // location of the item from the cloud 
    index: prop<number>(),
    // indicates that it's been saved to the layout panel
    saved: prop<boolean>(false),
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

