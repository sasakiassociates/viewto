import { Store } from '@strategies/stores';
import { reaction, observable, action } from 'mobx';
import { Model, model, prop } from 'mobx-keystone';

import { Project } from './Project';
import { ViewStudy } from './ViewStudy';

import { View } from './View';
import { PointView } from './View';
import { Speckle } from '@strategies/speckle'
import { Explorer } from './Explorer';


@model("viewto/Scenario")
export class Scenario extends Model({
    views: prop<View[]>(() => []),
    views: prop<PointView[]>(() => []),
    project: prop<Project>(() => new Project({})),
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

    @observable
    explorer?: Explorer;

    @action
    setExplorer(obj: Explorer) {
        this.explorer = obj;
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
        reaction(
            () => [this.study?.hasLoaded],
            () => {
                if (!this.study || this.study.isLoading || !this.study.hasLoaded || !this.study.getActiveResultCloud) {
                    return;
                }
                this.setExplorer(new Explorer(this.study))
                this.explorer?.setCondition(this.study?.focuses[0].sasakiId, this.study?.obstructors[0].sasakiId)
            }
        )
    }

    private async _loadStudyFromProject() {
        console.log('scenario loading');

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



