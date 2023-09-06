import { action, computed, makeObservable, observable } from 'mobx';
import { stores } from '@strategies/stores';
import Stores from '../stores/Stores';
import { Scene } from 'three';

export class Explorer {
    /**
     *
     */
    constructor() {
        makeObservable(this);
    }

    @computed
    get study() {
        return (stores as Stores).scenario.study;
    }

    @computed
    get cloud() {
        return this.study?.getActiveResultCloud;
    }

    @computed
    get view() {
        return (stores as Stores).views.active;
    }

    @computed
    get hasLoaded() {
        return this.study?.hasLoaded && this.cloud && this.cloud.values;
    }

    @computed
    get colors(): string[] {
        return this.cloud ? this.cloud.values.map(x => this.view?.getColorByValue(x)) : [];
    }

    @action
    setScene(scene: Scene) {
        if (this.scene?.id !== scene.id) this.scene = scene;
    }

    @observable
    scene?: Scene;
}
