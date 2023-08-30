import { ResultCloud } from './ResultCloud';
import { ExplorerDataModifierSupreme } from './ExplorerDataModifierSupreme';
import { clamp, getMinMax, normalise } from '../packages/ExplorerUtils/ArrayCommands';

export class Explorer {

    cloud: ResultCloud;
    modifiers: ExplorerDataModifierSupreme;
    /**
     *
     */
    constructor(cloud: ResultCloud, modifiers: ExplorerDataModifierSupreme | undefined = undefined) {
        this.cloud = cloud;
        this.modifiers = modifiers !== undefined ? modifiers : new ExplorerDataModifierSupreme();
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
    public valuesById(focus: string, obstructor: string, raw: boolean = false): number[] {
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

        if (raw) return values;

        const clamped = clamp(values, this.modifiers.solRange.min, this.modifiers.solRange.max)
        const minMax = getMinMax(clamped);
        return normalise(clamped, minMax[0], minMax[1]);
    }

    /**
     * Returns a list of hex based colors from a list of colors
     *
     * @param array - a set of normalized values from 0-1
     * @returns a list of colors 
     *
     */
    public colorsByValue(array: number[]): string[] {

        let values: string[] = [];

        if (!array) {
            console.warn('Values are not able to be used');
            return values;
        }
        return array.map(x => this.modifiers.gradient.Color(x));
    }


}