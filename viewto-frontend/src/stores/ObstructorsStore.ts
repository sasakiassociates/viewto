import { Store, stores } from '@strategies/stores';
import { computed, makeObservable } from 'mobx';

import Stores from '../stores/Stores';
import { Dict } from '../util';
import { ObstructingContext } from '../models/Context';


export default class ObstructorsStore extends Store {

    constructor() {
        super();
        makeObservable(this);
    }

    @computed
    get all(): ObstructingContext[] {
        return (stores as Stores).scenario.study?.obstructors || [];
    }

    @computed
    get byId(): Dict<ObstructingContext> {
        return this.all.reduce((a, b) => ({ ...a, [b.sasakiId]: b }), {});
    }

    @computed
    get byName(): Dict<ObstructingContext> {
        return this.all.reduce((a, b) => ({ ...a, [b.name]: b }), {});
    }

}
