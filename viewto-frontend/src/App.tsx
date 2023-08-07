import MenuBar from './components/MenuBar/MenuBar';
import LayersPanel from './components/LayersPanel/LayersPanel';
import SelectionPanel from './components/SelectionPanel/SelectionPanel';
import PointViewPanel from './components/PointViewPanel/PointViewPanel';
import Viewer from './components/Viewer/Viewer';
import { ObjectConverter } from './converter/ObjectConverter';

export default function App() {
    const loader = ObjectConverter();
    return (
        <div className="ViewTo">
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
