import { ViewDataModifierSupreme } from './ViewDataModifierSupreme';
import { computed, action, makeObservable, observable } from 'mobx';
import { remapSols } from '../packages/ExplorerUtils/DataMapping';
import { ViewStudy } from './ViewStudy';


export class Explorer {

    study: ViewStudy;
    modifiers: ViewDataModifierSupreme;

    /**
     *
     */
    constructor(study: ViewStudy, modifiers: ViewDataModifierSupreme | undefined = undefined) {
        makeObservable(this);
        this.study = study;
        this.modifiers = modifiers !== undefined ? modifiers : new ViewDataModifierSupreme();
    }



    @observable
    values?: number[]


    @action
    setActiveValues(data: number[]) {
        this.values = data;
    }

    @observable
    colors?: string[]

    @computed
    get cloud() {
        return this.study?.getActiveResultCloud;
    }

    @computed
    get hasLoaded() {
        return this.study?.hasLoaded && this.cloud && this.values;
    }

    /**
     * Gets a single result value that match the names of conditions
     *
     * @param focus - the name of the focus context to look for
     * @param obstructor - the name of the obstructor to look for
     * @returns returns the raw value of the result data
     *
     */
    public setCondition(focus: string, obstructor: string) {
        let values: number[] = [];

        if (!this.cloud) {
            console.warn('No cloud is set to this explorer');
            return values;
        }

        console.log(`View Condition Input\nFocus:${focus}\nObstructor:${obstructor}`);

        for (let i = 0; i < this.cloud.results.length; i++) {
            const condition = this.cloud.results[i].condition;

            if (condition.focusId !== focus || condition.obstructorId !== obstructor) continue

            values = [...this.cloud.results[i].values];
            break;
        }

        this.values = remapSols(values, this.modifiers.solRange.min, this.modifiers.solRange.max);
        this.colors = this.values?.map(x => this.modifiers.gradient.Color(x));
    }

}