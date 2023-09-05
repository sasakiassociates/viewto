import { Store, stores } from '@strategies/stores';
import { computed, makeObservable } from 'mobx';

import Stores from '../stores/Stores';
import { Dict } from '../util';
import { ObstructContext } from '../models/Context';


export default class ObstructorsStore extends Store {

    constructor() {
        super();
        makeObservable(this);
    }

    @computed
    get all(): ObstructContext[] {
        return (stores as Stores).scenario.study?.obstructors || [];
    }

    @computed
    get byId(): Dict<ObstructContext> {
        return this.all.reduce((a, b) => ({ ...a, [b.sasakiId]: b }), {});
    }

    @computed
    get byName(): Dict<ObstructContext> {
        return this.all.reduce((a, b) => ({ ...a, [b.name]: b }), {});
    }

}
