import { FocusContext, ObstructingContext } from './Context';
import { ViewResult } from './ViewCondition';



export class ViewStudy {
    focuses: FocusContext[];
    obstructors: ObstructingContext[];
    clouds: string[];
    results: ViewResult[];
};
