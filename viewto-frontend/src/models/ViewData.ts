import { Model, model, prop } from 'mobx-keystone';
import { FocusContext, ObstructingContext } from './Context';


@model("viewto/ViewDataOption")
export class ViewDataCondition extends Model({
    // list of focus items linked to this view 
    focuses: prop<FocusContext[]>(() => []),
    // list of conditions items linked to this view 
    obstructors: prop<ObstructingContext[]>(() => []),
}) {
}


@model("viewto/ViewDataFilter")
export class ViewDataFilter extends Model({
    // an array of two that can range to set the max and min pixel counts    
    pixelRange: prop<number[]>(() => [0.25, 0.75]),
    // an array of two that can range to set the max and min of the normalized values    
    valueRange: prop<number[]>(() => [0.2, 0.8]),
}) {
}