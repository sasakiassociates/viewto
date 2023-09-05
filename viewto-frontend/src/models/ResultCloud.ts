import { ViewResult } from './ViewResult';

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
        this.id = id;
        this.sasakiId = sasakiId;
        this.points = points;
        this.results = results;
        this.active = false;
    }
}
