import { Model, model,prop } from 'mobx-keystone';

// this is a similar object from the c# side where the result option represents the combination of focuses and conditions 
// this option is the final layer of meta data before accessing the result data values
@model("viewto/Condition")
export class ViewCondition extends Model({
    // id of this sasaki object
    focus: prop<string>(),
    // reference id to the speckle object
    obstructor: prop<string[]>(),
    // an enum value that says what stage (potential, existing, or proposed) was captured in  
    type: prop<string>()
}) {}


export type ViewResult = {
    values : number[],
    condition : ViewCondition 
} 
