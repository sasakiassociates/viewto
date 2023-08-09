// take in a list and have an offset or reduce by function

export function arrayReduce(offset:number, array:number[]):number[] {
    // 1. loop over array and grab each offset number
    let results = [];
    for (let i = 0; i < array.length; i+=offset) {
        //const element = array[i];
        results.push(array[i]);
    }
    return results;
}

// get min & max
export function getMinMax(array:number[]):number[] {
    // 1. create arrays to store min and max
    let min = array[0];
    let max = array[0];
    for (let i = 0; i < array.length; i++) {
        min = Math.min(min, array[i]);
        max = Math.max(max, array[i]);
    }
    return [min,max];
}


// normalise values
export function normalise(array:number[], min:number, max:number, log:number):number[] {
    let results = [];
    for (let i = 0; i < array.length; i++) {
        results[i] = (array[i] - min) / (max - min);
    }
    return results;
}

// filter
export function setScoreSelection(array:number[], scoreMin:number, scoreMax:number):number[] {
    let results = [];
    for (let i = 0; i < array.length; i++) {
        // 1. check the number is within the bounds of min/max value
        if(array[i] >= scoreMin && array[i] <= scoreMax) {
            results.push(array[i]);
        }
    }
    return results;
}

// average array
export function arrayAverage(offset:number, array:number[]):number[] {
    // 1. loop over array and grab each offset number
    let results = [];
    for (let i = 0; i < array.length; i+=offset) {
        // 2. gather points between
        let val = 0;
        for (let j = 0; j < offset; j++) {
            val += array[i+j];
        }
        val /= offset;
        results.push(val);
    }
    return results;
}
