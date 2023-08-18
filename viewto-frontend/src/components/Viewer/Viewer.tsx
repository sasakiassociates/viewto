import { Panel, Title } from '@strategies/ui';
import { observer } from 'mobx-react';
import { Canvas } from '@react-three/fiber';
import CameraControl from './CameraControl';

export default observer(function Viewer() {
    return (
        <Panel className="Viewer" active={true}>
            <Title>View</Title>
            <Canvas>
                {/* <CameraControl /> */}
                <mesh>
                    <torusKnotGeometry />
                    <meshNormalMaterial />
                </mesh>
            </Canvas>
        </Panel>
    );
});
