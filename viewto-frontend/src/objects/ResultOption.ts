
export class ViewDataInfo {
    // id of this sasaki object
    focusId: string;
    // reference id to the speckle object
    conditionId: string[];
    // an enum value that says what stage (potential, existing, or proposed) was captured in  
    stage: ResultStage;
}

export class ViewData {
    // the information related to this series of data pieces
    info: ViewDataInfo;
    // the values associated with them
    values: number[]
}


export enum ResultStage {
    "Potential", "Existing", "Proposed"
}