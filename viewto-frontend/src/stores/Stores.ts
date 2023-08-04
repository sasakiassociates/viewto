import { register } from '@strategies/stores';

import UIStore from './UIStore';


type Stores = {
    ui: UIStore;
};


export function registerStores() {
    register({
        ui: new UIStore(),
    });
}

export default Stores;
