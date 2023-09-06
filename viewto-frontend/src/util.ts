import { observable } from "mobx";

export type Dict<T = any> = { [key: string]: T };

export function noop() {}

/* missing stuff from dmo */
export function getMinMax(array: number[]): number[] {
    let min = Number.MAX_SAFE_INTEGER;
    let max = 0;
    for (let i = 0; i < array.length; i++) {
        if (array[i] < min) min = array[i];
        else if (array[i] > max) max = array[i];
    }
    return [min, max];
}

// normalise values
export function normalise(array: number[], min: number, max: number): number[] {
    let results = [];
    for (let i = 0; i < array.length; i++) {
        results[i] = (array[i] - min) / (max - min);
    }
    return results;
}

export function clamp(array: number[], min: number, max: number): number[] {
    let results = [...array];

    for (let i = 0; i < array.length; i++) {
        if (array[i] < min) results[i] = min;
        else if (array[i] > max) results[i] = max;
    }
    return results;
}


export class Range {
    constructor(min: number, max: number) {
        this.min = min;
        this.max = max;
    }
    @observable
    min: number = 0;

    @observable
    max: number = 1;
}
