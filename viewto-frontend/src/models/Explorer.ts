import { ResultCloud } from './ResultCloud';
import { ConditionType, ViewCondition } from './ViewCondition'
import { ExplorerDataModifierSupreme } from './ExplorerDataModifierSupreme';
import { clamp, getMinMax, normalise } from '../packages/ExplorerUtils/ArrayUtility';

export class Explorer {

    cloud: ResultCloud;
    modifiers: ExplorerDataModifierSupreme;
    /**
     *
     */
    constructor(cloud: ResultCloud, modifiers: ExplorerDataModifierSupreme | undefined) {
        this.cloud = cloud;
        this.modifiers = modifiers ? modifiers : new ExplorerDataModifierSupreme();
    }

    /**
     * Gets a single result value that match the names of conditions
     *
     * @param focus - the name of the focus context to look for
     * @param obstructor - the name of the obstructor to look for
     * @param raw - optional toggle for passing back the raw pixel values or using the modifiers to normalize them
     * @returns returns the raw value of the result data
     *
     */
    public getValuesById(focus: string, obstructor: string, type: string, raw: boolean = false): number[] {
        let values: number[] = [];

        if (!this.cloud) {
            console.warn('No cloud is set to this explorer');
            return values;
        }

        for (let i = 0; i < this.cloud.results.length; i++) {
            const condition = this.cloud.results[i].condition;
            if (condition.focusId == focus && condition.obstructorId == obstructor && condition.type == type) {
                values = this.cloud.results[i].values;
                break;
            }

        }
        // return them raw dog
        if (raw) return values;

        // clamp the values by our global min max range (remember, this is for saying what is the range of pixels we want to consider ) 
        const clamped = clamp(values, this.modifiers.solRange.min, this.modifiers.solRange.max)
        // grab this lists min and max (this might be something we don't need to do)
        const minMax = getMinMax(clamped);
        // normalize those values 
        return normalise(clamped, minMax[0], minMax[1]);
    }


}