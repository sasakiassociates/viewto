import { observer } from 'mobx-react';
import { DebugViewer as SpeckleViewer, ViewerEvent } from '@speckle/viewer';
import { useRef, useEffect } from 'react';
import { stores, useStores } from '@strategies/stores';
import Stores from '../../stores/Stores';
import * as THREE from 'three';
import { Explorer } from '../../models/Explorer';
import { ConditionType } from '../../models/ViewCondition';

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
        // check if null
        if (!viewer.current) return;

        // the async call to load each item into the speckle viewer
        (async () => {
            try {
                if (!scenario.study || scenario.study.isLoading || !scenario.study.hasLoaded) {
                    console.log('study is not ready for the viewer');
                    return;
                }
                //#region load the geometry into the viewer

                // get all of the objects we need to stream in
                const references = scenario.study.getAllReferences;
                console.log('refernces', references);

                // go through each one version ref to pull in
                for await (const versionRef of references) {
                    const url = `https://sasaki.speckle.xyz/streams/${scenario.project.id}`;
                    const objUrl = `${url}/objects/${versionRef.referenceObject}`;

                    await viewer.current?.loadObjectAsync(
                        objUrl,
                        import.meta.env.VITE_SPECKLE_TOKEN,
                        enableCaching,
                        priorty,
                        zoomFit
                    );
                }

                console.log('study loaded into viewer');

                //#endregion

                //#region Map Values through explorer

                // create a new exploer to handle the data processing
                const explorer = new Explorer(scenario.study?.results[0]);

                // hard code the condition we want to setup
                const focus = focuses.all[0];
                const obstructor = obstructors.all[0];

                const values = explorer.valuesById(focus.sasakiId, focus.sasakiId);
                const colors = new Float32Array(values.length * bufferItemSize);

                const threeColors = explorer.colorsByValue(values).map(x => {
                    return new THREE.Color(x);
                });

                for (let i = 0; i < threeColors.length; i++) {
                    const i3 = i * 3;
                    colors[i3 + 0] = threeColors[i].r;
                    colors[i3 + 1] = threeColors[i].g;
                    colors[i3 + 2] = threeColors[i].b;
                }

                //#endregion

                //#region post viewer setup

                // relocating the camera to the point cloud
                const cloudIds = viewer.current
                    ?.getDataTree()
                    .findAll((guid, obj) => {
                        return obj.speckle_type === 'Objects.Geometry.Pointcloud';
                    })
                    .map(x => x.id as string);
                viewer.current?.zoom(cloudIds);

                // find the three point cloud and modify its colors
                const worldTree = viewer.current?.getWorldTree();
                const renderTree = worldTree?.getRenderTree();
                const renderViews = renderTree?.getRenderableRenderViews(SpeckleType.Pointcloud);
                if (renderViews === undefined) return;

                /* 
                NOTE: this is using the DebugViewer from the @speckle/viewer
                post the explains the solution 
                https://speckle.community/t/accessing-threejs-objects-through-viewer/6897/3?u=haitheredavid

                there are some steps that we are assuming here in order to get three js geo 
                this solution connects us to the point cloud object that we need to modify from the ui 
                */

                // assume we want this point cloud
                const pointCloud = renderViews[0];

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

                //#endregion
            } catch (error) {
                console.error(error);
            }
        })();
    }, [scenario.study?.hasLoaded]);

    return <div className="Viewer" ref={viewerRef} />;
});
