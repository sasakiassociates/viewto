import { FocusContext, ObstructingContext } from './Context';
import { ViewCloud } from './ViewCloud';
import { ViewCondition, ConditionTypeLookUp } from './ViewCondition';
import { ViewResult } from './ViewResult';


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
    results: ViewResult[];

    constructor(data: any) {
        this.id = data.id;
        this.name = data.ViewName;
        this.sasakiId = data.ViewId;
        this.focuses = data.objects
            .filter(x => x.speckle_type == ViewObjectTypes.context.speckle_type && x.Content_Type == "Potential")
            .map(x => this._focusContextToWeb(x));
        this.obstructors = data.objects
            .filter(x => x.speckle_type == ViewObjectTypes.context.speckle_type && x.Content_Type != "Potential")
            .map(x => this._obstructorContextToWeb(x));
        this.clouds = data.objects
            .filter(x => x.speckle_type == ViewObjectTypes.cloud.speckle_type)
            .map(x => this._viewCloudToWeb(x));
        this.results = data.objects
            .filter(x => x.speckle_type == ViewObjectTypes.result.speckle_type)
            .map(x => this._viewResultsToWeb(x))

    }


    get getPointCloud(){
        return this.clouds.map(cld => cld.id);
    }
    
    // grabs all of the view context objects references 
    get getSpeckleMeshes() {
        return [...this.focuses, ...this.obstructors].map(ctx => ctx.references).reduce((a, b) => [...a, ...b])
    }

    // conversions for getting Focus Context from speckle to app
    _focusContextToWeb(obj: any): FocusContext {
        return new FocusContext(obj.ViewName, obj.ViewId, [obj.id])
    }

    // conversions for getting Obstructors from speckle to app
    _obstructorContextToWeb(obj: any): ObstructingContext {
        return new ObstructingContext(obj.ViewName, obj.ViewId, [obj.id], (obj.Content_Type === "Proposed"));
    }

    // conversions for getting View Cloud from speckle to app
    _viewCloudToWeb(obj: any): ViewCloud {
        return new ViewCloud(obj.id, obj.Positions);
    }

    // conversions for getting View Result DAta from speckle to app    
    _viewResultsToWeb(obj: any): ViewResult {
        return new ViewResult(obj.values, this._viewConditionToWeb(obj.Target_Id, obj.Content_Id, obj.ViewContentType));
    }
    // conversions for getting View Condition from speckle to app
    _viewConditionToWeb(focus: string, obstructor: string, type: string): ViewCondition {
        return new ViewCondition(focus, obstructor, ConditionTypeLookUp[type])
    }

}

