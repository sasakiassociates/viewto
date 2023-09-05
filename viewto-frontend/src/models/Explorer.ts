import { ViewDataModifierSupreme } from './ViewDataModifierSupreme';
import { computed, action, makeObservable, observable } from 'mobx';
import { ViewStudy } from './ViewStudy';
import { FocusContext, ObstructContext } from './Context';

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

    @computed
    get cloud() {
        return this.study?.getActiveResultCloud;
    }

    @observable
    focuses?: FocusContext[] = [];

    @action
    setFocuses(focuses: FocusContext[]) {
        this.focuses = focuses;
    }

    @observable
    obstructors?: ObstructContext[] = [];

    @action
    setObstructors(obstructors: ObstructContext[]) {
        this.obstructors = obstructors;
    }

    @computed
    get hasLoaded() {
        return this.study?.hasLoaded && this.cloud && this.values;
    }

    @computed
    get canExplore() {
        return this.hasLoaded && this.focuses && this.focuses.length > 0;
    }

    @computed
    get values(): number[] {
        let rawValues: number[] = [];

        if (this.cloud === undefined) {
            console.warn('Explorer is not ready to genereate values');
            return rawValues;
        }

        this.focuses?.forEach(focus => {
            const focusId = focus.sasakiId;
            // note: when there is no obstructor object we set use the focus id as the obstructor and focus item in the condition.
            const obstructorIds =
                this.obstructors && this.obstructors.length > 0
                    ? this.obstructors.map(x => x.sasakiId)
                    : [focus.sasakiId];

            console.log('obstructor ids', obstructorIds);

            obstructorIds?.forEach(obstructorId => {
                let conditionRawValues: number[] = [];

                for (let i = 0; i < this.cloud!.results.length; i++) {
                    const condition = this.cloud!.results[i].condition;

                    if (condition.Equals(focusId, obstructorId)) continue;

                    conditionRawValues = clamp(
                        [...this.cloud!.results[i].values],
                        this.modifiers.solRange.min,
                        this.modifiers.solRange.max
                    );
                    break;
                }
                if (rawValues.length == 0) {
                    rawValues = [...conditionRawValues];
                } else {
                    for (let i = 0; i < rawValues.length; i++) {
                        rawValues[i] += conditionRawValues[i];
                    }
                }
                for (let i = 0; i < 10; i++) {
                    console.log(rawValues[i]);
                }
            });
        });

        const clampedMinMax = getMinMax(rawValues);
        return normalise(rawValues, clampedMinMax[0], clampedMinMax[1]);
    }

    @computed
    get colors(): string[] {
        return this.values?.map(x => this.modifiers.getColorByValue(x));
    }
}

