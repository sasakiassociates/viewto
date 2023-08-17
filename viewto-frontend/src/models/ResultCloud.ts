import { ViewResult } from './ViewResult';


export class ResultCloud {
    id: string;
    sasakiId: string;
    points: number[];
    results: ViewResult[];

    /**
     *
     */
    constructor(id: string, sasakiId: string, points: number[], results: ViewResult[]) {
        this.id = id;
        this.sasakiId = sasakiId;
        this.points = points;
        this.results = results;
    }
}
