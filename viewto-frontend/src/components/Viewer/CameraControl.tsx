import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls.js';
import { extend, useFrame, useThree } from '@react-three/fiber';

extend({ OrbitControls });

export default function CameraControl() {
    const { camera, gl } = useThree();
    console.log(camera);

    // @ts-ignore
    return <orbitControls arg={[camera, gl.domElement]} />;
}
