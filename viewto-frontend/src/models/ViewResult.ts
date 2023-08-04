import { Model, model, prop } from 'mobx-keystone';
import { ViewCondition } from './ViewCondition';



@model("viewto/Results")
export class ViewResult extends Model({
    values: prop<number[]>(() => []),
    condition: prop<ViewCondition>()
}) {
}
