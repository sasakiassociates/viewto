import { register } from '@strategies/stores';

import UIStore from './UIStore';
import PointsStore from './PointsStore';
import { Scenario } from '../models/Scenario';
import Config from '../config';
import ViewsStore from "./ViewsStore";
import FocusesStore from "./FocusesStore";


type Stores = {
    config: Config; 
    focuses: FocusesStore;
    points: PointsStore;
    scenario: Scenario;
    ui: UIStore;
    views:ViewsStore;
};


export function registerStores() {
    register({
        config: new Config(),
        focuses: new FocusesStore(),
        points: new PointsStore(),
        scenario: new Scenario({}),
        ui: new UIStore(),
        views: new ViewsStore()
    });
}

export default Stores;
