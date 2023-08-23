import { observer } from 'mobx-react';
import { Viewer as SpeckleViewer, ViewerEvent } from '@speckle/viewer';
import { useRef, useEffect } from 'react';
import { useStores } from '@strategies/stores';
import Stores from '../../stores/Stores';

// api docs for viewer
// https://speckle.notion.site/Viewer-API-Documentation-11f7bcbf3d2547c2985b0c988fb9889e
export default observer(function Viewer() {
    const { scenario } = useStores<Stores>();

    const viewerRef = useRef<HTMLDivElement>(null);
    const viewer = useRef<SpeckleViewer>();

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
            console.log('load progress', arg);
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

                /* load the geometry into the viewer */

                // get all of the objects we need to stream in
                const references = scenario.study.getAllReferences;
                console.log('refernces', references);

                const enableCaching = true;
                const priorty = 1;
                const zoomFit = false;

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

                /* adjust the camera view to focus on the point cloud */

                // copy the tree from the viewer
                const tree = viewer.current?.getDataTree();
                // serach for our point cloud object
                const clouds = tree?.findAll((guid, obj) => {
                    return obj.speckle_type === 'Objects.Geometry.Pointcloud';
                });

                // sad
                if (clouds === undefined) return;

                // zoom into the our point cloud
                const ids = clouds.filter(x => typeof x.id === 'string').map(o => o.id as string);
                viewer.current?.zoom(ids);
            } catch (error) {
                console.error(error);
            }
        })();
    }, [scenario.study?.hasLoaded]);

    return <div className="Viewer" ref={viewerRef} />;
});
