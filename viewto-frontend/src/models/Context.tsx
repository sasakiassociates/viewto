
import { Model, computedTree, model, prop } from 'mobx-keystone';

// a simple reference object used to keep track of the speckle reference id and the sasaki app id  
@model("viewto/Context")
export class Context extends Model({
    // name of the context object
    name: prop<string>(),
    // id of this sasaki object
    sasakiId: prop<string>(),
    // reference id to the speckle object
    referenceId: prop<string[]>()
}) { }


// similar to the context class but with a specific type declared for view studies
export class FocusContext extends Model({
    // name of the context object
    name: prop<string>(),
    // id of this sasaki object
    sasakiId: prop<string>(),
    // reference id to the speckle object
    referenceId: prop<string[]>()
}) { }

// similar to the context class but can be a toggle for the proposed context type
export class ConditionContext extends Model({
    // name of the context object
    name: prop<string>(),
    // id of this sasaki object
    sasakiId: prop<string>(),
    // reference id to the speckle object
    referenceId: prop<string[]>(),
    // toggle for declaring what group the obstructor is
    proposed: prop<boolean>(false)
}) {

    @computedTree
    isProposed(proposed: boolean = true) {
        this.proposed = proposed;
    }
}


