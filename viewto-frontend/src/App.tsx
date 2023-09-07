import { useEffect } from 'react';
import { stores, useStores } from '@strategies/stores';

import Stores from './stores/Stores';
import MenuBar from './components/MenuBar/MenuBar';
import LayersPanel from './components/LayersPanel/LayersPanel';
import SelectionPanel from './components/SelectionPanel/SelectionPanel';
import PointViewPanel from './components/PointViewPanel/PointViewPanel';
import StartupModal from './components/StartupModal/StartupModal';
import Viewer from './components/Viewer/Viewer';


export default function App() {
    const { ui } = useStores<Stores>();

    useEffect(() => {
        // @ts-ignore
        window.stores = stores; 
    }, []);

    return (
        <div className="ViewTo" ref={el => el && !ui.appRef && ui.setAppRef(el)}>
            {/* <StartupModal /> */}

            <div className="ViewTo__layout">
                <MenuBar />
                <LayersPanel />
                <SelectionPanel />
                <PointViewPanel />
                <Viewer />
            </div>
        </div>
    );
}
