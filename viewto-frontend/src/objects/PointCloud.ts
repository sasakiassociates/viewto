import { CloudPoint } from "./CloudPoint";
import { SasakiObject } from "./SasakiObject";


export class PointCloud implements SasakiObject {
    sasakiId: string;
    points: CloudPoint[];
}
