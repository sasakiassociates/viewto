import { Model, model, prop } from 'mobx-keystone';
import { FocusContext, ObstructingContext } from './Context';
import { ViewCloud } from './ViewCloud';
import { ViewResult } from './ViewResult';


type easyName  ={
    speckle_type: string;}

export  class ViewObjectTypes {
    static cloud: easyName = { speckle_type: "ViewObjects.Speckle.ViewCloudReference"};
    static study: easyName = { speckle_type: "ViewObjects.Speckle.ViewStudy"};
    static context: easyName = { speckle_type: "ViewObjects.Speckle.ContentReference"};
    static result: easyName = { speckle_type: "ViewObjects.Speckle.ResultCloud"};
}


@model("viewto/ViewStudy")
export class ViewStudy extends Model({
    id:prop<string>(""),
    sasakiId: prop<string>(""),
    name: prop<string>(""),
    focuses:prop<FocusContext[]>(() => []),
    obstructors:prop<ObstructingContext[]>(() => []),
    clouds: prop<ViewCloud[]>(() => []),
    results: prop<ViewResult[]>(() => [])
}) {

    toString(options?: { withData?: boolean; }): string;
    toString(): string;
    toString(options?: unknown): string {
        return `ViewStudy ${this.name} with ${(this.clouds.length +this.focuses.length + this.obstructors.length )} objects` ;
   
    }
}
