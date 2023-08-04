import { register } from '@strategies/stores';

import UIStore from './UIStore';
import PointsStore from './PointsStore';
import { Scenario } from '../models/Scenario';
import Config from '../config';


type Stores = {
    config: Config; 
    points: PointsStore;
    scenario: Scenario;
    ui: UIStore;
};


export function registerStores() {
    register({
        config: new Config(),
        points: new PointsStore(),
        scenario: new Scenario({}),
        ui: new UIStore(),
    });
}

export default Stores;
