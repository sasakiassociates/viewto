import { Model, prop } from 'mobx-keystone';

// this is a similar object from the c# side where the result option represents the combination of focuses and conditions 
// this option is the final layer of meta data before accessing the result data values
export class ResultOption extends Model({
    // id of this sasaki object
    focusId: prop<string>(),
    // reference id to the speckle object
    conditionId: prop<string[]>(),
    // an enum value that says what stage (potential, existing, or proposed) was captured in  
    stage: prop<string>()
}) {}
