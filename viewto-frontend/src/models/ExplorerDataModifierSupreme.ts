import { Gradient, Virdis } from "../packages/ExplorerUtils/Gradient";

export class ExplorerDataModifierSupreme {

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
     * max=8191
     */
    solRange: Range;

    /**
     * the range of values when visualizing the normalized data
     * 
     * @default
     * min=0  
     * max=1   
    */
    valueRange: Range;

    /**
     * a list of colors organized to make a happy lil gradient
     * 
     */
    gradient: Gradient;

    /*
     */
    constructor() {
        // hard coded value for max int 
        this.solRange = new Range(0, 2147483647);
        this.valueRange = new Range(0, 1);
        this.gradient = new Virdis();
    }

}

export class Range {
    min: number;
    max: number;
    /**
     *
     */
    constructor(min: number, max: number) {
        this.min = min;
        this.max = max;
    }
}