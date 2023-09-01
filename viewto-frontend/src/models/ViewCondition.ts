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

    Equals(focusId: string, obstructorId: string): boolean {
        return this.focusId === focusId && this.obstructorId === obstructorId;
    }
}

export enum ConditionType {
    POTENTIAL = "Potential", EXISTING = "Existing", PROPOSED = "Proposed"
}

export const ConditionTypeLookUp: { [key: string]: ConditionType } = {
    potential: ConditionType.POTENTIAL,
    existing: ConditionType.EXISTING,
    proposed: ConditionType.PROPOSED
}


