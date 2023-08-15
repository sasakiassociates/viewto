import { ViewCondition } from './ViewCondition';

export class ViewResult {
    values: number[];
    condition: ViewCondition;

    constructor(values : number[], condition : ViewCondition){
        this.values = values;
        this.condition = condition;
    }
}
