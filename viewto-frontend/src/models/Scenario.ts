import { Store } from '@strategies/stores';
import { reaction, observable, action } from 'mobx';
import { Model, model, prop } from 'mobx-keystone';


import { Project } from './Project';
import { ViewStudy } from './ViewStudy';

import ObjectLoader from "@speckle/objectloader";
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
                console.log('study reaction');
                this._loadStudyModel();
            }
        );

        reaction(
            () => this.project?.id,
            () => {
                console.log('project reaction');
                this._loadStudy(study => this.setStudy(study));
            }
        );
    }


    private _loadStudy(fn: (data: ViewStudy) => void) {
        // the scenario model references the speckle project(stream) that we need to pull the referene object from the database
        // the ui will give us the input for this project and object
        // for now we have a hard coded test project in place
        console.log(`Loading new Project: ${this.project.id}`);

        const loader = new ObjectLoader({
            // @ts-ignore
            token: import.meta.env.VITE_SPECKLE_TOKEN,
            // @ts-ignore
            serverUrl: import.meta.env.VITE_SPECKLE_URL,
            // @ts-ignore
            streamId: import.meta.env.VITE_VIEWTO_TEST_PROJECT,
            // @ts-ignore
            objectId: import.meta.env.VITE_VIEWTO_TEST_STUDY,
            // options: {
            //     fullyTraverseArrays: false, // Default: false. By default, if an array starts with a primitive type, it will not be traversed. Set it to true if you want to capture scenarios in which lists can have intersped objects and primitives, e.g. [ 1, 2, "a", { important object } ]
            //     excludeProps: ["displayValue", "displayMesh", "__closure"], // Default: []. Any prop names that you pass in here will be ignored from object construction traversal.
        })

        const referenceObj = loader.getAndConstructObject((e) => {
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
        this.study.getSpeckleMeshes.map(reference => {
            const loader = new ObjectLoader({
                // @ts-ignore
                token: import.meta.env.VITE_SPECKLE_TOKEN,
                // @ts-ignore
                serverUrl: import.meta.env.SPECKLE_URL,
                streamId: this.project.id,
                objectId: reference
            });

            const referenceObj = loader.getAndConstructObject((e) => {
                // event loop for getting progress on the loading
                console.log("Progress ", e.stage, ":", e.current / e.total);
            });

            // this is now just rendering data that we want in the viewer
            // @ts-ignore
            return referenceObj.Data
        })
    };
}



