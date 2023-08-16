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
        if (this.project) {
            this._loadStudyFromProject(study => this.setStudy(study));
        }
        reaction(
            () => [this.project?.id],
            () => {
                console.log('Project id change:', this.project);
                // NOTE: if we get a new project id, is it safe to assume that models and versions have been selected as well? That info is needed to load the study
                // for now I just broke it into separate parts for testing  
            }
        );
        reaction(
            () => [this.project?.model],
            () => {
                console.log('Project model change:', this.project);
                // NOTE:  we still need to select a version from this model to find and load the commit from 
            }
        );

        reaction(
            () => [this.project?.version],
            () => {
                console.log('Project version change:', this.project);
                // NOTE: this is where we could possibly load a study into the dashboard
                // this._loadStudy(study => this.setStudy(study));
            }
        );


        reaction(
            () => [this.study?.id],
            () => {
                console.log('study id has been changed', this.study);
                this._loadStudyReferences((reference) => { console.log(reference) });
            }
        );
    }


    private async _loadStudyFromProject(fn: (data: ViewStudy) => void) {
        // the scenario model references the speckle project(stream) that we need to pull the referene object from the database
        // the ui will give us the input for this project and object
        // for now we have a hard coded test project in place
        console.log(`Loading new Project: ${this.project.name}\n${this.project.id}\n${this.project.model}\n${this.project.version}`);

        // NOTE: we can hard code the commit reference object for making sure we get all the view study data but we will need to find the reference object for each item in the study  

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

        const referenceObj = await loader.getAndConstructObject((e) => {
            // event loop for getting progress on the loading
            // console.log("Progress ", e.stage, ":", e.current / e.total);
        });

        // @ts-ignore
        const study = new ViewStudy(referenceObj.Data);
        fn(study);

    };

    private async _loadStudyReferences(fn: (reference: string) => void) {
        console.log(`Loading new View Study: ${this.study.name}`);
        // 1. load in all of the geometry references

        // we get every mesh from the reference objects and load them into the scene 
        this.study.getContextReferences.map(reference => {

            console.log(`Loading new reference ${reference}`);
            // TODO: Implement client for fetching id of commit object
            return;
            // create a new loader for each object 
            const loader = new ObjectLoader({
                // @ts-ignore
                token: import.meta.env.VITE_SPECKLE_TOKEN,
                // @ts-ignore
                serverUrl: import.meta.env.SPECKLE_URL,

                streamId: this.project.id,
                objectId: reference
            });

            // TODO: implement batching for await during map 
            const referenceObj = await loader.getAndConstructObject((e) => {
                // event loop for getting progress on the loading
                console.log("Progress ", e.stage, ":", e.current / e.total);
            });

            // let whatever called this function get a callback for the new reference being loaded
            fn(reference);

            // this is now just rendering data that we want in the viewer            
        })

        // 2. load the point cloud
        // 3. apply the results

        // 4. end 

    };
}



