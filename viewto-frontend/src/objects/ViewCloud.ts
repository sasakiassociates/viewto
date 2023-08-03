
import { PointCloud } from "./PointCloud";
import { ViewData } from "./ResultOption";

export class ViewCloud extends PointCloud {

    // the values associated with the points in the cloud
    results: ViewData[];
    hasResults = () => this.results != null || this.results.length > 0;
}
