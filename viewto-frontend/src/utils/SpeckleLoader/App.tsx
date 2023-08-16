import {observer} from "mobx-react";
import React, {useRef, useState, Suspense, MutableRefObject} from 'react'
import MainStore from "./store/MainStore";
import {Object3D, Vector3} from 'three';
import {LoadObjects} from "./LoadObjects";

declare type ObjectRef = MutableRefObject<Object3D>;

export type MainStoreProps = {
    store: MainStore;
}

const App = ({store}: MainStoreProps) => {

    const [hovered, onHover] = useState<ObjectRef | null>(null)
    const selected = hovered ? [hovered] : undefined

    return (
        <div className="App">
            <button onClick={() => {
                new LoadObjects();
            }}>LOAD
            </button>
        </div>
    );
};

export default observer(App);
