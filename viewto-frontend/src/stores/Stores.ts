import { register } from '@strategies/stores';

import UIStore from './UIStore';
import PointsStore from './PointsStore';
import { Scenario } from '../models/Scenario';


type Stores = {
    points: PointsStore;
    scenario: Scenario;
    ui: UIStore;
};


export function registerStores() {
    register({
        points: new PointsStore(),
        scenario: new Scenario({}),
        ui: new UIStore(),
    });
}

export default Stores;
