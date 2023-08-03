import { Model, model, prop } from 'mobx-keystone';
import { Point } from './Point';
import { FocusContext, ConditionContext } from './Context';

@model("viewto/View")
export class View extends Model({
    // readable name of the view  
    name: prop<string>(),
    // generated id of view
    id: prop<string>(),
    // referenced point the view was created
    point: prop<Point>(),
    // properties of the camerea details 
    // TODO: figure out what properties we need to store
    camera: prop<string>(),
    // list of focus items linked to this view 
    focuses: prop<FocusContext[]>(),
    // list of conditions items linked to this view 
    condition: prop<ConditionContext>(),
    // an array of two that can range to set the max and min pixel counts    
    pixelRange: prop<number[]>(() => [0.25, 0.75]),
    // an array of two that can range to set the max and min of the normalized values    
    valueRange: prop<number[]>(() => [0.20, 0.80]),
    // a blob png of the view 
    texture: prop<string>(),
}) {
    // suppose to be volatile 
    active: boolean;
}

@model("viewto/View")
export class ExplorerSettings extends Model({
    // list of focuses 
    focuses: prop<FocusContext[]>(),
    // list of conditions items linked to this view 
    condition: prop<ConditionContext>(),
    // an array of two that can range to set the max and min pixel counts    
    pixelRange: prop<number[]>(() => [0.25, 0.75]),
    // an array of two that can range to set the max and min of the normalized values    
    valueRange: prop<number[]>(() => [0.20, 0.80]),
}) {}

