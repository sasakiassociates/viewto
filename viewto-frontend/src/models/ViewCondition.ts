// this is a similar object from the c# side where the result option represents the combination of focuses and conditions 
// this option is the final layer of meta data before accessing the result data values
export class ViewCondition {
    // id of this sasaki object
    focusId: string;
    // reference id to the speckle object
    obstructorId: string;
    // an enum value that says what stage (potential, existing, or proposed) was captured in  
    type: ConditionType;

    /**
     *
     */
    constructor(focusId: string, obstructorId: string, type: ConditionType) {
        this.focusId = focusId;
        this.obstructorId = obstructorId;
        this.type = type;
    }
}

export enum ConditionType {
    POTENTIAL, EXISTING, PROPOSED
}

export const ConditionTypeLookUp: { [key: string]: ConditionType } = {
    Potential: ConditionType.POTENTIAL,
    Existing: ConditionType.EXISTING,
    Proposed: ConditionType.PROPOSED
}

