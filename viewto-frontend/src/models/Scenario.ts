import { Store } from '@strategies/stores';
import { reaction, observable, action } from 'mobx';
import { Model, model, prop } from 'mobx-keystone';

import { Project } from './Project';
import { ViewStudy } from './ViewStudy';

import { View } from './View';
import { Speckle } from '@strategies/speckle'

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

        const speckle = new Speckle({
            // @ts-ignore
            token: import.meta.env.VITE_SPECKLE_TOKEN,
            // @ts-ignore
            server: import.meta.env.VITE_SPECKLE_URL
        })

        // not really necessary, but this is a simple way to make sure we have an authenticated
        console.log(await speckle.activeUser);

        /* 
         the scenario model references the speckle project(stream) that we need to pull the referene object from the database
         the ui will give us the input for this project and object
         for now we have a hard coded test project in place
        */

        // console.log(`Loading new Project: ${this.project.name}\n${this.project.id}\n${this.project.model}\n${this.project.version}`);

        // load the version (commit) data
        // @ts-ignore
        const version = await speckle.Project(import.meta.env.VITE_VIEWTO_TEST_PROJECT).Version(import.meta.env.VITE_VIEWTO_TEST_VERSION).get;
        console.log(version);

        // load the reference object from the version (commit)
        // @ts-ignore
        const referenceObj = await speckle.Project(import.meta.env.VITE_VIEWTO_TEST_PROJECT).Object(version.referencedObject).get;
        console.log(referenceObj);

        // deconstruct all the view study data
        // @ts-ignore
        const study = new ViewStudy(referenceObj.Data);
        // call back for async function
        fn(study);
    };

    private async _loadStudyReferences(fn: (reference: string) => void) {
        console.log(`Loading new View Study: ${this.study.name}`);

        const speckle = new Speckle({
            // @ts-ignore
            token: import.meta.env.VITE_SPECKLE_TOKEN,
            // @ts-ignore
            server: import.meta.env.VITE_SPECKLE_URL
        })

        const loadVersion = async (reference: string, type: string) => {
            console.log(`Loading new ${type} : ${reference}`);
            // @ts-ignore
            const obj = await speckle.Project(import.meta.env.VITE_VIEWTO_TEST_PROJECT).Version(reference).get;
            //TODO: this is where we laod the viewer data into speckle
            console.log(obj);

        }


        // 1. load in all of the geometry references
        // we get every mesh from the reference objects and load them into the scene 
        this.study.getContextReferences.map(ref => loadVersion(ref, "tbd"));
        this.study.getCloudReferences.map(cld => loadVersion(cld, "tbd"));

        // 2. load the point cloud
        // 3. apply the results

        // 4. end 

    };
}



