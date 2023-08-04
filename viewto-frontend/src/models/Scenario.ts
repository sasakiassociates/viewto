import { Store } from '@strategies/stores';
import { action, computed, observable, reaction } from 'mobx';
import { Model, model, prop } from 'mobx-keystone';

import { Project } from './Project';
import { View } from './View';
import { Point } from './Point';
import { FocusContext, ObstructingContext } from './Context';

import ObjectLoader, { SpeckleObject } from "@speckle/objectloader";
import { ViewCondition } from './ViewCondition';
import { ViewResult } from './ViewResult';
import { ViewStudy, ViewObjectTypes } from './ViewStudy';
import { ViewCloud } from './ViewCloud';


@model("viewto/Scenario")
export class Scenario extends Model({
    // the information linked to the speckle project loaded
    project: prop<Project>(() => new Project({})),
    // the list of views stored to apply the settings 
    views: prop<View[]>(() => []),
}) implements Store {

    onRegister() {}
    onUpdateRegistration() {}

    // returns the list of points found in the views 
    @computed
    get points(): Point[] {
        return this.views.map((x) => x.point);
    }

    // gets all the focuses from the views saved
    @computed
    get focuses(): FocusContext[] {
        const data = [] as FocusContext[];
        this.views.map((x) => x.condition.focuses.map((child) => data.push(child)));
        return data;
    }

    // get all the obstructing conditions from the views
    @computed
    get conditions(): ObstructingContext[] {
        const data = [] as ObstructingContext[];
        this.views.map((x) => x.condition.obstructors.map((child) => data.push(child)));
        return data
    }

    @observable
    data: any;


    @action
    setData(data: any) {
        this.data = data;
    }

    @observable
    canLoad: boolean;

    @action
    setLoad(canLoad: boolean) {
        this.canLoad = canLoad;
    }

    onInit() {

        if(this.project){
            this._loadData();
        }
        reaction(
            () => [this.project.id],
            () =>{
                this._loadData();
            } 
            );

    }


    // this needs to be fetched in the authentaticaiton process
    private static token = "089149f2176eb6b5ba309f011ca24bcfc32376998e";
    // this should come from the login process
    private static url = "https://speckle.xyz";
    // comes from the commit selection 
    private static objectRef = "f4b16ebe7e93ea3ec653cd284d72ca05";

    private _loadData() {
        console.log(`Loading new Project: ${this.project.id}`);
    
        const loader = new ObjectLoader({
            token: Scenario.token,
            serverUrl: Scenario.url,
            streamId: this.project.id,
            objectId: Scenario.objectRef,
            // options: {
            //     fullyTraverseArrays: false, // Default: false. By default, if an array starts with a primitive type, it will not be traversed. Set it to true if you want to capture scenarios in which lists can have intersped objects and primitives, e.g. [ 1, 2, "a", { important object } ]
            //     excludeProps: ["displayValue", "displayMesh", "__closure"], // Default: []. Any prop names that you pass in here will be ignored from object construction traversal.
            // },
        });
    
        const getObjectAndManuallyConvert = async (): Promise<SpeckleObject | SpeckleObject[]> => {
        
            const referenceObj = await loader.getAndConstructObject((e) => {
                // event loop for getting progress on the loading
                console.log("Progress ", e.stage, ":", e.current / e.total);
        });

        
        // deconstructing the speckle object
        // @ts-ignore
        const data = referenceObj.Data;

        // the conversions from speckle to keystone
        const study = new ViewStudy({
            id: data.id,
            name: data.ViewName,
            sasakiId : data.ViewId,
            focuses : data.objects
                .filter(x => x.speckle_type == ViewObjectTypes.context.speckle_type && x.Content_Type == "Potential")
                .map(x => this._focusContextToWeb(x)),
            obstructors: data.objects
                .filter(x => x.speckle_type == ViewObjectTypes.context.speckle_type && x.Content_Type != "Potential")
                .map(x => this._obstructorContextToWeb(x)),
            clouds: data.objects
                .filter(x => x.speckle_type == ViewObjectTypes.cloud.speckle_type )
                .map(x => this._viewCloudToWeb(x)),
            results: data.objects
                .filter(x => x.speckle_type == ViewObjectTypes.result.speckle_type)
                .map(x => this._viewResultsToWeb(x))
        })

        return referenceObj;
    };
    getObjectAndManuallyConvert();
    }
    
    // conversions for getting Focus Context from speckle to app
    _focusContextToWeb(obj : any) : FocusContext{
        return new FocusContext({
            name : obj.ViewName,
            sasakiId: obj.ViewId,
            referenceId: obj.id,  
        })    
    }

    // conversions for getting Obstructors from speckle to app
    _obstructorContextToWeb(obj : any) : ObstructingContext{
        return new ObstructingContext({
            name : obj.ViewName,
            sasakiId: obj.ViewId,
            referenceId: obj.id,
            proposed: (obj.Content_Type === "Proposed")  
        })    
    }

    // conversions for getting View Cloud from speckle to app
    _viewCloudToWeb(obj :any): ViewCloud{
        return new ViewCloud({sasakiId: obj.ViewId, id:obj.id, points: obj.Positions});
    }

    // conversions for getting View Result DAta from speckle to app    
    _viewResultsToWeb(obj : any) : ViewResult{
        return new ViewResult({values: obj.values, 
            condition: this._viewConditionToWeb(obj.Target_Id, obj.Content_Id, obj.ViewContentType) });
    }

    // conversions for getting View Condition from speckle to app
    _viewConditionToWeb(focus: string, obstructor:string ,type:string ): ViewCondition{
        return new ViewCondition({type: type, focus: focus, obstructor: obstructor})
    }
}



