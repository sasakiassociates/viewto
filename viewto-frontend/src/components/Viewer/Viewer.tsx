import { observer } from 'mobx-react';
import { DebugViewer as SpeckleViewer, ViewerEvent } from '@speckle/viewer';
import { useRef, useEffect } from 'react';
import { useStores } from '@strategies/stores';
import Stores from '../../stores/Stores';
import * as THREE from 'three';

enum SpeckleType {
    View3D = 'View3D',
    BlockInstance = 'BlockInstance',
    Pointcloud = 'Pointcloud',
    Brep = 'Brep',
    Mesh = 'Mesh',
    Point = 'Point',
    Line = 'Line',
    Polyline = 'Polyline',
    Box = 'Box',
    Polycurve = 'Polycurve',
    Curve = 'Curve',
    Circle = 'Circle',
    Arc = 'Arc',
    Ellipse = 'Ellipse',
    RevitInstance = 'RevitInstance',
    Text = 'Text',
    Unknown = 'Unknown',
}

// api docs for viewer
// https://speckle.notion.site/Viewer-API-Documentation-11f7bcbf3d2547c2985b0c988fb9889e
export default observer(function Viewer() {
    const { scenario, focuses, obstructors } = useStores<Stores>();

    const viewerRef = useRef<HTMLDivElement>(null);
    const viewer = useRef<SpeckleViewer>();

    const enableCaching = true;
    const priorty = 1;
    const zoomFit = false;
    const bufferItemSize = 3;

    useEffect(() => {
        if (viewer.current) return;
        viewer.current = new SpeckleViewer(viewerRef.current!);

        /* events that our ui might want to show */
        viewer.current.on(ViewerEvent.DownloadComplete, arg => {
            console.log('dowload complete', arg);
        });
        viewer.current.on(ViewerEvent.LoadComplete, arg => {
            console.log('load complete', arg);
        });
        viewer.current.on(ViewerEvent.LoadProgress, arg => {
            // console.log('load progress', arg);
        });
        viewer.current.on(ViewerEvent.ObjectClicked, arg => {
            console.log('object clicked', arg);
        });
        viewer.current.on(ViewerEvent.ObjectDoubleClicked, arg => {
            console.log('object double clicked', arg);
        });

        (async () => {
            await viewer.current?.init();
        })();
    }, []);

    // this effect will load each version reference from the study and then load it to the viewer
    useEffect(() => {
        if (!viewer.current) return;

        (async () => {
            try {
                if (!scenario.study || scenario.study.isLoading || !scenario.study.hasLoaded) {
                    console.log('study is not ready for the viewer');
                    return;
                }
                //#region load the geometry into the viewer

                // go through each one version ref to pull in
                for await (const versionRef of scenario.study.getAllReferences) {
                    await viewer.current?.loadObjectAsync(
                        `https://sasaki.speckle.xyz/streams/${scenario.project.id}/objects/${versionRef.referenceObject}`,
                        import.meta.env.VITE_SPECKLE_TOKEN,
                        enableCaching,
                        priorty,
                        zoomFit
                    );
                }

                // relocating the camera to the point cloud
                const cloudIds = viewer.current
                    ?.getDataTree()
                    .findAll((guid, obj) => {
                        return obj.speckle_type === 'Objects.Geometry.Pointcloud';
                    })
                    .map(x => x.id as string);

                viewer.current?.zoom(cloudIds);

                console.log(`${scenario.study.name} is loaded into the viewer`);

                //#endregion
            } catch (error) {
                console.error(error);
            }
        })();
    }, [scenario.study?.hasLoaded]);

    // effect for mapping the active colors from the explorer
    useEffect(() => {
        console.log('Effect from viewer with color changing');

        if (!scenario.explorer?.colors) {
            console.log('no values here');
            return;
        }

        const colors = new Float32Array(scenario.explorer.colors.length * bufferItemSize);

        const threeColors = scenario.explorer.colors.map(x => {
            return new THREE.Color(x);
        });

        for (let i = 0; i < threeColors.length; i++) {
            const i3 = i * 3;
            colors[i3 + 0] = threeColors[i].r;
            colors[i3 + 1] = threeColors[i].g;
            colors[i3 + 2] = threeColors[i].b;
        }

        //#region post viewer setup

        // find the three point cloud and modify its colors
        const worldTree = viewer.current?.getWorldTree();
        const renderTree = worldTree?.getRenderTree();
        const renderViews = renderTree?.getRenderableRenderViews(SpeckleType.Pointcloud);
        if (renderViews === undefined) return;

        // assume we want this point cloud
        const pointCloud = renderViews[0];

        // TODO: chat with the boys about communicating back to the scenario that the viewer is loaded and we can find this id
        if (!pointCloud) {
            console.warn('no point cloud found yet');
            return;
        }

        // the three object we find from the scene with the batch id
        const threeObj = viewer.current
            ?.getRenderer()
            .scene.getObjectByProperty('uuid', pointCloud.batchId);

        // NOTE: necessary to do this or point cloud wont should different vertex colors
        // @ts-ignore
        threeObj.material[0].vertexColors = true;

        // the geometry where all of the the point cloud data is stored
        // @ts-ignore
        const geometry = threeObj.geometry;

        // appl the new color values to the geometry and trigger an update
        geometry.setAttribute('color', new THREE.BufferAttribute(colors, 3));
        geometry.attributes.color.needsUpdate = true;
    }, [scenario.explorer?.colors]);

    return <div className="Viewer" ref={viewerRef} />;
});
