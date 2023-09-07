import { stores } from '@strategies/stores';
import Stores from '../stores/Stores';
import { ViewResult } from './ViewResult';
import { computed, makeObservable } from 'mobx';
import { clamp, getMinMax, norm, normalise, normaliseArray } from '../util';
import { ConditionType, ViewCondition } from './ViewCondition';
import { Range } from '../util';

export class ResultCloud {
    id: string;
    sasakiId: string;
    points: number[];
    results: ViewResult[];
    active: boolean;
    resultCount: number;

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
        this.resultCount = results[0].values.length;
    }

    @computed
    get values(): number[] {
        const view = (stores as Stores).views.active;

        if (!view.conditions || view.conditions.length === 0) {
            return this._blankArrayFromCount();
        }
        console.log('conditions', view.conditions);
        

        const maxValues = this._compositeConditionValues(
            view.conditions.filter(x => x.type === ConditionType.POTENTIAL),
            view.solRange
        );

        // no obstructors so lets just pass it back 
        if(view.obstructors.length === 0) return norm(maxValues);

        const minValues = this._blankArrayFromCount();

        const inputValues = this._compositeConditionValues(
            view.conditions.filter(x => x.type !== ConditionType.POTENTIAL),
            view.solRange
        );

        return normaliseArray(inputValues, minValues, maxValues);
    }

    private _compositeConditionValues(
        conditions: ViewCondition[],
        bounds: Range,
        clampValues: boolean = false
    ): number[] {
        let compositeValues = this._blankArrayFromCount();

        for (let i = 0; i < conditions.length; i++) {
            let values = this._blankArrayFromCount();

            for (let l = 0; l < this.results.length; l++) {
                const layer = this.results[l];

                if (layer.condition.Equals(conditions[i].focusId, conditions[i].obstructorId)) {
                    values = clampValues
                        ? clamp([...layer.values], bounds.min, bounds.max)
                        : [...layer.values];
                    break;
                }
            }

            for (let i = 0; i < compositeValues.length; i++) {
                compositeValues[i] += values[i];
            }
        }

        return compositeValues;
    }

    private _blankArrayFromCount(value: number = 0): number[] {
        return new Array(this.resultCount).fill(value);
    }
}
