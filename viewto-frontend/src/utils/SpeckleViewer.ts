import {
    Group,
    Mesh,
    MeshNormalMaterial,
    Loader,
    InterpolateSmooth
} from 'three';
import ViewerObjectLoader from "./SpeckleLoader/modules/ViewerObjectLoader";

class SpeckleLoader extends Loader {

    viewerObjectLoader : ViewerObjectLoader;

    load(options, onLoad, onProgress, onError) {
        const {token, objectUrl} = JSON.parse(options);

        this.viewerObjectLoader = new ViewerObjectLoader(objectUrl, token);

        const container = new Group();

        const material = new MeshNormalMaterial();

        const addObject = (o) => {
            const mesh = new Mesh(o.bufferGeometry, material);
            // mesh.geometry.computeFaceNormals();
            // mesh.geometry.computeVertexNormals();
            mesh.material.flatShading = true;
            container.add(mesh);
        }

        const onProgressInfo = (progress) => {//TODO not sure what structure the onProgress and onError for the threejs loaders expect
            console.log('onProgress', progress)
        }
        const onErrorInfo = (progress) => {
            console.log('onError', progress)
        }

        this.viewerObjectLoader.load(addObject, onProgressInfo, onErrorInfo).then(() => {
            onLoad(container);
        });
    }
}