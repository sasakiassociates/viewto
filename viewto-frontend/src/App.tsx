import { useEffect } from 'react';
import { stores } from '@strategies/stores';

import MenuBar from './components/MenuBar/MenuBar';
import LayersPanel from './components/LayersPanel/LayersPanel';
import SelectionPanel from './components/SelectionPanel/SelectionPanel';
import PointViewPanel from './components/PointViewPanel/PointViewPanel';
import Viewer from './components/Viewer/Viewer';
import FiberViewer from './components/FiberViewer/FiberViewer';

export default function App() {
    useEffect(() => {
        // @ts-ignore
        window.stores = stores;
    }, []);

    return (
        <div className="ViewTo">
            <div className="ViewTo__layout">
                <MenuBar />
                <LayersPanel />
                <SelectionPanel />
                <PointViewPanel />
                <Viewer />
                <FiberViewer />
            </div>
        </div>
    );
}
