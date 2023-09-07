import { Model, model, prop } from 'mobx-keystone';
import { action, computed, observable } from 'mobx';
import { stores } from '@strategies/stores';
import Stores from '../stores/Stores';
import chroma, { Scale } from 'chroma-js';
import { Range } from '../util';
import { ConditionType, ViewCondition } from './ViewCondition';

@model('viewto/View')
export class View extends Model({
    focusIds: prop<string[]>(() => []).withSetter(),
    obstructorIds: prop<string[]>(() => []).withSetter(),
}) {
    @computed
    get focuses() {
        return this.focusIds.map(id => (stores as Stores).focuses.byId[id]);
    }

    @computed
    get obstructors() {
        return this.obstructorIds.map(id => (stores as Stores).obstructors.byId[id]);
    }

    @computed
    get conditions() {
        let conditions: ViewCondition[] = [];

        if (!this.focuses || this.focuses.length === 0) {
            console.log('no focuses selected');
            return conditions;
        }

        this.focuses.forEach(focus => {
            conditions.push(
                new ViewCondition(focus.sasakiId, focus.sasakiId, ConditionType.POTENTIAL)
            );
        });

        if (!this.obstructors || this.obstructors.length === 0) {
            console.log('no obstructors selected');
            return conditions;
        }

        this.obstructors.forEach(obstructor => {
            this.focuses.forEach(focus => {
                conditions.push(new ViewCondition(focus.sasakiId, obstructor.sasakiId, obstructor.proposed ? ConditionType.PROPOSED : ConditionType.EXISTING))
            })
        })
        return conditions;
    }

    @observable
    active: boolean = false;

    @action
    setActive(active: boolean) {
        this.active = active;
    }

    /**
     * the range of sols(pixels) to bound the result data
     *
     * @description
     * A sol is metric for counting the amount of fragments(pixels) that a view has.
     *
     * The main difference between the value between the fragment count and a sol is
     * that a sol accounts for the coordinate location of the fragment and creates a value for that fragment
     * depending on it's location in the camera. This gives a higher value to fragment that is centered in our view
     * vs one that is floating in the corner somewhere.
     *
     * 8191 is the max amount of sols a single camera can have. Most of the data created have 6 viewers total
     *
     * @default
     * min=0
     * max=2147483647
     */
    solRange: Range = new Range(0, 2147483647);

    /**
     * the range of values when visualizing the normalized data
     *
     * @default
     * min=0
     * max=1
     */
    valueRange: Range = new Range(0, 1);

    /**
     * a list of colors organized to make a happy lil gradient
     *
     */
    gradient: Scale = chroma.scale('viridis');

    getColorByValue(value: number): string {
        return this.gradient
            .domain([this.valueRange.min, this.valueRange.max])(this._clamp(value, 0, 1))
            .hex();
    }

    private _clamp(value: number, min: number, max: number) {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
}
