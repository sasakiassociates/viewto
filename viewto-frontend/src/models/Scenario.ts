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
}



