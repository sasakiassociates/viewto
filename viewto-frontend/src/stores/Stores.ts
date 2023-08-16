import { register } from '@strategies/stores';

import UIStore from './UIStore';
import PointsStore from './PointsStore';
import { Scenario } from '../models/Scenario';
import Config from '../config';
import ViewsStore from "./ViewsStore";


type Stores = {
    config: Config; 
    points: PointsStore;
    scenario: Scenario;
    ui: UIStore;
    view:ViewsStore;
};


export function registerStores() {
    register({
        config: new Config(),
        points: new PointsStore(),
        scenario: new Scenario({}),
        ui: new UIStore(),
        view: new ViewsStore()
    });
}

export default Stores;
