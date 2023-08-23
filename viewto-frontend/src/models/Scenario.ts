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

    readonly speckle = new Speckle({
        token: import.meta.env.VITE_SPECKLE_TOKEN,
        server: import.meta.env.VITE_SPECKLE_URL
    });

    onRegister() { }
    onUpdateRegistration() { }

    @observable
    study?: ViewStudy;

    @action
    setStudy(data: any) {
        this.study = data;
    }

    onInit() {
        if (this.project && this.project.complete) {
            (async () => {
                this.setStudy(await this._loadStudyFromProject());
            })();
        }

        reaction(
            () => [this.project?.key],
            async () => {
                if (this.project.complete) {
                    this.setStudy(await this._loadStudyFromProject());
                }
            }
        );

        /*
        reaction(
            () => [this.study?.id],
            () => {
                console.log('study id has been changed', this.study);
                this._loadStudyReferences((reference) => { console.log(reference) });
            }
        );
        */
    }


    private async _loadStudyFromProject() {
        // not really necessary, but this is a simple way to make sure we have an authenticated
        console.log(await this.speckle.activeUser);

        // load the version (commit) data
        const version = await this.speckle.Project(import.meta.env.VITE_VIEWTO_TEST_PROJECT).Version(import.meta.env.VITE_VIEWTO_TEST_VERSION).get;
        console.log(version);

        // load the reference object from the version (commit)
        const referenceObj = await this.speckle.Project(import.meta.env.VITE_VIEWTO_TEST_PROJECT).Object(version.referencedObject).get;
        console.log(referenceObj);

        // deconstruct all the view study data
        // @ts-ignore
        return new ViewStudy(referenceObj.Data);
    };
}



