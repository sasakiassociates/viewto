import { Store, stores } from '@strategies/stores';
import { computed, makeObservable } from 'mobx';

import Stores from '../stores/Stores';
import { Dict } from '../util';
import { FocusContext } from '../models/Context';


export default class FocusesStore extends Store {

    constructor() {
        super();
        makeObservable(this);
    }

    @computed
    get all(): FocusContext[] {
        return (stores as Stores).scenario.study?.focuses || [];
    }

    @computed
    get byId(): Dict<FocusContext> {
        return this.all.reduce((a,b) => ({ ...a, [b.sasakiId]: b }), {});
    }

    @computed
    get byName(): Dict<FocusContext> {
        return this.all.reduce((a,b) => ({ ...a, [b.name]: b }), {});
    }

}
