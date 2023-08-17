import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls.js';
import { extend, useFrame, useThree } from '@react-three/fiber';

// should automatically extend the three.js class into jsx
extend({ OrbitControls });

export default function CameraControl() {
    const { camera, gl } = useThree();
    return <orbitControls arg={[camera, gl.domElement]} />;
}
