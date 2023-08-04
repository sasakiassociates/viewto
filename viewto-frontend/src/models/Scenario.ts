import { Store } from '@strategies/stores';
import { action, computed, observable, reaction } from 'mobx';
import { Model, model, prop } from 'mobx-keystone';

import { Project } from './Project';
import { View } from './View';
import { Point } from './Point';
import { FocusContext, ObstructingContext } from './Context';

import ObjectLoader, { SpeckleObject } from "@speckle/objectloader";
import { ViewCondition } from './ViewCondition';
import { ViewStudy } from './ViewStudy';

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

    _focusContextToWeb(obj : any) : FocusContext{

        return new FocusContext({
            name : obj.ViewName,
            sasakiId: obj.ViewId,
            referenceId: obj.id,
            
        })
        
    }
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
            // console.log("Progress ", e.stage, ":", e.current / e.total);
        });

        
        // deconstructing the speckle object
        // @ts-ignore
        const viewStudy = referenceObj.Data;
        console.log(viewStudy.ViewName + "{" + viewStudy.objects.length + "}\n");
        console.log(viewStudy);

                
        // grabbing the view clouds
        const cloud = viewStudy.objects.filter(x => x.speckle_type == "ViewObjects.Speckle.ViewCloudReference");
        console.log("cloud count=", cloud.length);

        // grabbing the context items for geometry
        const context = viewStudy.objects.filter(x => x.speckle_type == "ViewObjects.Speckle.ContentReference")
        console.log("context count=", context.length);


        // const vs : ViewStudy = {
        //     clouds: viewStudy.objects.filter(x => x.speckle_type == "ViewObjects.Speckle.ViewCloudReference"),
        //     focuses: context.filter(x => x.Content_Type === "Potential").map(x => this._focusContextToWeb),
        //     // const existing = context.filter(x => x.Content_Type === "Existing");
        //     // const proposed = context.filter(x => x.Content_Type === "Proposed");
        // }
        


        // filtering out the different context within the study
        const targets = context.filter(x => x.Content_Type === "Potential");
        const existing = context.filter(x => x.Content_Type === "Existing");
        const proposed = context.filter(x => x.Content_Type === "Proposed");

        console.log("targets=", targets.length);
        console.log("existing=", existing.length);
        console.log("proposed=", proposed.length);

        /* 
        the result cloud contains all of the data we need to do the analysis exploration
        we just end up using the context pieces from the view study to load the geometry  
         */

        // grabbing the result cloud
        const result = viewStudy.objects.filter(x => x.speckle_type === "ViewObjects.Speckle.ResultCloud");
        console.log("result count=", result.length);

        const resultCloud = result[0];
        console.log("Result Cloud: ", resultCloud);

        // grabbing the point data from the cloud
        const points = resultCloud.Positions;
        console.log('point count', points.length / 3);

        // grabbing the view data from the cloud
        const viewData = resultCloud.Data;
        console.log('view data count:', viewData.length);



        // reading through the view conditions of the data
        const conditions= viewData.map(x => {
            return new ViewCondition({
                focus: x.Target_Id,
                obstructor: x.Content_Id,
                type: x.ViewContentType 
            }) 

            
        })

        conditions.map(x => {
            console.log(
            `View Data Condition\nType: ${x.type}\nTarget: ${x.focus}\nObstructor: ${x.obstructor}` );
    })


        return referenceObj;
    };
    getObjectAndManuallyConvert();
    
    }
}



