import { stores } from '@strategies/stores';
import Stores from '../stores/Stores';
import { ViewResult } from './ViewResult';
import { computed, makeObservable } from 'mobx';
import { clamp, getMinMax, normalise } from '../util';


export class ResultCloud {
    id: string;
    sasakiId: string;
    points: number[];
    results: ViewResult[];
    active: boolean;

    /**
     *
     */
    constructor(id: string, sasakiId: string, points: number[], results: ViewResult[]) {
        makeObservable(this);

        this.id = id;
        this.sasakiId = sasakiId;
        this.points = points;
        this.results = results;
        this.active = false;
    }

    @computed
    get values(): number[] {
        const view = (stores as Stores).views.active;

        let rawValues: number[] = [];
        const obstructors = view.obstructors;

        view.focuses?.forEach(focus => {
            const focusId = focus.sasakiId;
            
            // note: when there is no obstructor object we set use the focus id as the obstructor and focus item in the condition.
            const obstructorIds =
                obstructors && obstructors.length > 0
                    ? obstructors.map(x => x.sasakiId)
                    : [focus.sasakiId];

            console.log('obstructor ids', obstructorIds);

            obstructorIds?.forEach(obstructorId => {
                let conditionRawValues: number[] = [];

                for (let i = 0; i < this.results.length; i++) {
                    const condition = this.results[i].condition;

                    if (condition.Equals(focusId, obstructorId)) continue;

                    conditionRawValues = clamp(
                        [...this.results[i].values],
                        view.solRange.min,
                        view.solRange.max
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
}
