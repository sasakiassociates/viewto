import { SasakiObject } from "./SasakiObject";

export type point3 = {
    x: number;
    y: number;
    z: number;
}

export class CloudPoint implements SasakiObject {
    sasakiId: string;
    point: point3;
    vector: point3;
    meta: string = "";
}
