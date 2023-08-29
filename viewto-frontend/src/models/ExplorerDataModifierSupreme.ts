export class ExplorerDataModifierSupreme {

    // the range of sols(pixels) to bound the result data 
    solRange: Range;

    // the range of values when visualizing the normalized data 
    valueRange: Range;

    // the array of colors to map too
    gradient: Gradient;
    
    /**
     *
     */
    constructor() {
        this.solRange = new Range(0, Number.MAX_SAFE_INTEGER);
        this.valueRange = new Range(0, 1);
    }

}

export class Gradient {
    // some list of values for gradients
    // todo: swtich to keys or something for more customization
    colors: number[]

    // stupid way of getting the color from the index value
    // todo: fix this shit 
    color(value: number): number {
        return this.colors[value];
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