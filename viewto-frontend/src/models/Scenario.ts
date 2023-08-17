import { Store } from '@strategies/stores';
import { reaction, observable, action } from 'mobx';
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


    private async _loadStudy(fn: (data: ViewStudy) => void) {
        console.log(`Loading new Project: ${this.project.id}`);

        const viewStudyExampleReference = "f4b16ebe7e93ea3ec653cd284d72ca05";

        const loader = new ObjectLoader({
            // @ts-ignore
            token: import.meta.env.SPECKLE_TOKEN,
            // @ts-ignore
            serverUrl: import.meta.env.SPECKLE_URL,
            streamId: this.project.id,
            objectId: this.project.version,
            // options: {
            //     fullyTraverseArrays: false, // Default: false. By default, if an array starts with a primitive type, it will not be traversed. Set it to true if you want to capture scenarios in which lists can have intersped objects and primitives, e.g. [ 1, 2, "a", { important object } ]
            //     excludeProps: ["displayValue", "displayMesh", "__closure"], // Default: []. Any prop names that you pass in here will be ignored from object construction traversal.
        })

        const referenceObj = await loader.getAndConstructObject((e) => {
            // event loop for getting progress on the loading
            console.log("Progress ", e.stage, ":", e.current / e.total);
        });

        // @ts-ignore
        fn(new ViewStudy(referenceObj.Data))
    };

    private _loadStudyModel() {
        console.log(`Loading new View Study: ${this.study.name}`);

        // deconstructing the speckle object
        // we get every mesh from the reference objects and load them into the scene 
        this.study.getSpeckleMeshes.map(async reference => {
            const loader = new ObjectLoader({
                // @ts-ignore
                token: import.meta.env.SPECKLE_TOKEN,
                // @ts-ignore
                serverUrl: import.meta.env.SPECKLE_URL,
                streamId: this.project.id,
                objectId: reference
            });

            const referenceObj = await loader.getAndConstructObject((e) => {
                // event loop for getting progress on the loading
                console.log("Progress ", e.stage, ":", e.current / e.total);
            });

            // this is now just rendering data that we want in the viewer
            // @ts-ignore
            return referenceObj.Data
        });
    };
}



