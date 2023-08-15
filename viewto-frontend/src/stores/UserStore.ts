import { Store, stores } from '@strategies/stores';
import { action, computed, observable, makeObservable } from 'mobx';
import { getApps, initializeApp } from 'firebase/app';
import { getAuth, onAuthStateChanged, signOut } from 'firebase/auth';

import Stores from './Stores';


export default class UserStore extends Store {

    constructor() {
        super();

        makeObservable(this);
    }

    onRegister() {
        if (getApps().length === 0) {
            initializeApp((stores as Stores).config.firebase);
        }

        onAuthStateChanged(getAuth(), (user: any) => {
            if (user) {
                this.setUser(user);
            }
            else {
                this.setUser(null);
            }
        });
    }

    @observable
    user: any = null;

    @computed
    get displayName() {
        return this.user ? this.user.displayName : '';
    }

    logout() {
        signOut(getAuth());
        window.location.reload();
    }

    @computed
    get isLoggedIn() {
        return this.user !== null;
    }

    @computed
    get isVerified() {
        return this.user && this.user.emailVerified;
    }

    @action
    setUser(user: any) {
        this.user = user !== null ? user : null;
    }

}
