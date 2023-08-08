import { Store } from '@strategies/stores';
import { action, observable, reaction } from 'mobx';
import { Model, model, prop } from 'mobx-keystone';

import { Project } from './Project';
import ObjectLoader from "@speckle/objectloader";
import { ViewStudy } from './ViewStudy';
import { View } from './View';

@model("viewto/Scenario")
export class Scenario extends Model({
    // the information linked to the speckle project loaded
    project: prop<Project>(() => new Project({})),
    // the list of views stored to apply the settings 
    views: prop<View[]>(() => []),
}) implements Store {

    onRegister() { }
    onUpdateRegistration() { }

    @observable
    study: ViewStudy;

    @action
    setStudy(data: any) {
        this.study = data;
    }

    onInit() {
        if (this.project) {
            this._loadStudy(study => this.setStudy(study));
        }

        reaction(
            () => [this.study?.id],
            () => {
                this._loadStudyModel();
            }
        );

        reaction(
            () => [this.project.id],
            () => {
                this._loadStudy(study => this.setStudy(study));
            }
        );
    }



    // this needs to be fetched in the authentaticaiton process
    private static token = "089149f2176eb6b5ba309f011ca24bcfc32376998e";
    // this should come from the login process
    private static url = "https://speckle.xyz";
    // comes from the commit selection 
    private static objectRef = "f4b16ebe7e93ea3ec653cd284d72ca05";

    private _loadStudy(fn: (data: ViewStudy) => void) {
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

        const getObjectAndManuallyConvert = async (): Promise<void> => {

            const referenceObj = await loader.getAndConstructObject((e) => {
                // event loop for getting progress on the loading
                console.log("Progress ", e.stage, ":", e.current / e.total);
            });

            // deconstructing the speckle object
            // @ts-ignore
            fn(new ViewStudy(referenceObj.Data));
        };

        getObjectAndManuallyConvert();
    }

    private _loadStudyModel() {
        console.log(`Loading new Study: ${this.study.name}`);

        // we get every mesh from the reference objects and load them into the scene 
        this.study.getSpeckleMeshes.map(reference =>{
            const loader = new ObjectLoader({
                token: Scenario.token,
                serverUrl: Scenario.url,
                streamId: this.project.id,
                objectId: reference  
            });

            const getObjectAndManuallyConvert = async (): Promise<void> => {

                const referenceObj = await loader.getAndConstructObject((e) => {
                    // event loop for getting progress on the loading
                    console.log("Progress ", e.stage, ":", e.current / e.total);
                });
    
                // this is now just rendering data that we want in the viewer
                // return referenceObj.Data 
            };
        })



    }

}



