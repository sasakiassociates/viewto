import { FocusContext, ObstructingContext } from './Context';
import { Point } from './Point';
import { ViewCloud } from './ViewCloud';
import { ViewCondition, ConditionTypeLookUp } from './ViewCondition';
import { ViewResult } from './ViewResult';
import { ResultCloud } from './ResultCloud';

import { computed, makeObservable, observable } from 'mobx';


type easyName = {
    speckle_type: string;
}

export class ViewObjectTypes {
    static cloud: easyName = { speckle_type: "ViewObjects.Speckle.ViewCloudReference" };
    static study: easyName = { speckle_type: "ViewObjects.Speckle.ViewStudy" };
    static context: easyName = { speckle_type: "ViewObjects.Speckle.ContentReference" };
    static result: easyName = { speckle_type: "ViewObjects.Speckle.ResultCloud" };
}

export class ViewStudy {
    id: string;
    sasakiId: string;
    name: string;
    focuses: FocusContext[];
    obstructors: ObstructingContext[];
    clouds: ViewCloud[];
    results: ResultCloud[];

    constructor(data?: any) {
        makeObservable(this);
        this.id = data.id;
        this.name = data.ViewName;
        this.sasakiId = data.ViewId;
        this.focuses = data.objects
            .filter((x: any) => x.speckle_type == ViewObjectTypes.context.speckle_type && x.Content_Type == "Potential")
            .map((x: any) => this._focusContextToWeb(x));
        this.obstructors = data.objects
            .filter((x: any) => x.speckle_type == ViewObjectTypes.context.speckle_type && x.Content_Type != "Potential")
            .map((x: any) => this._obstructorContextToWeb(x));
        this.clouds = data.objects
            .filter((x: any) => x.speckle_type == ViewObjectTypes.cloud.speckle_type)
            .map((x: any) => this._viewCloudToWeb(x));
        this.results = data.objects
            .filter((x: any) => x.speckle_type == ViewObjectTypes.result.speckle_type)
            .map((x: any) => this._resultCloudToWeb(x))
    }


    @observable
    points: Point[] = [];

    /*
    @computed
    get getCloudReferences() {
        const references = [];
        this.clouds.map(item => references.push(...item.references));
        return references;

    }

    @computed
    get getContextReferences() {
        return [...this.focuses, ...this.obstructors].map(ctx => ctx.references).reduce((a, b) => [...a, ...b])
    }
    */

    // conversions for getting Focus Context from speckle to app
    _focusContextToWeb(obj: any): FocusContext {
        return new FocusContext(obj.id, obj.ViewName, obj.ViewId, obj.References);
    }

    // conversions for getting Obstructors from speckle to app
    _obstructorContextToWeb(obj: any): ObstructingContext {
        return new ObstructingContext(obj.id, obj.ViewName, obj.ViewId, obj.References, (obj.Content_Type === "Proposed"));
    }

    // conversions for getting View Cloud from speckle to app
    _viewCloudToWeb(obj: any): ViewCloud {
        return new ViewCloud(obj.id, obj.References);
    }

    // conversions for getting all result cloud data     
    _resultCloudToWeb(obj: any): ResultCloud {
        return new ResultCloud(obj.id, obj.ViewId, obj.Positions, obj.Data.map((x: any) => this._viewResultToWeb(x)));
    }

    // conversions for result cloud data structure
    _viewResultToWeb(obj: any) {
        return new ViewResult(obj.values, this._viewConditionToWeb(obj.Target_Id, obj.Content_Id, obj.ViewContentType));
    }

    // conversions for getting View Condition from speckle to app
    _viewConditionToWeb(focus: string, obstructor: string, type: string): ViewCondition {
        return new ViewCondition(focus, obstructor, ConditionTypeLookUp[type.toLowerCase()])
    }

}

