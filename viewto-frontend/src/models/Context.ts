import {computed} from 'mobx';
import { Model, model, prop } from 'mobx-keystone';

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
@model("viewto/FocusContext")
export class FocusContext extends Model({
    // name of the context object
    name: prop<string>(),
    // id of this sasaki object
    sasakiId: prop<string>(),
    // reference id to the speckle object
    referenceId: prop<string[]>()
}) { }

// similar to the context class but can be a toggle for the proposed context type
@model("viewto/ObstructContext")
export class ObstructingContext extends Model({
    // name of the context object
    name: prop<string>(),
    // id of this sasaki object
    sasakiId: prop<string>(),
    // reference id to the speckle object
    referenceId: prop<string[]>(),
    // toggle for declaring what group the obstructor is
    proposed: prop<boolean>(false)
}) {
}


