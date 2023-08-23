import { observer } from 'mobx-react';
import { Viewer as SpeckleViewer } from '@speckle/viewer';
import { useRef, useEffect } from 'react';
import { useStores } from '@strategies/stores';
import Stores from '../../stores/Stores';

// api docs for viewer
// https://speckle.notion.site/Viewer-API-Documentation-11f7bcbf3d2547c2985b0c988fb9889e
export default observer(function Viewer() {
    const { scenario } = useStores<Stores>();

    // references for the component objects
    const viewerRef = useRef<HTMLDivElement>(null);
    const viewer = useRef<SpeckleViewer>();

    // effect=something that happens when state changes for this component
    // what they really mean= anything inside this effect, run whenever it's deps change
    // use effect only runs after being rendered the first time

    //  using an empty array means we have no deps, so we don't need to run it more than once

    useEffect(() => {
        if (viewer.current) return;

        viewer.current = new SpeckleViewer(viewerRef.current!);
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
                // this effect gets called before the study is set
                if (!scenario.study) {
                    console.log('Study has not been mounted to the scenario');
                    return;
                }

                if(!scenario.study.hasLoaded){
                    console.log('scenario has not loaded');
                    if(scenario.study.isLoading){
                        console.log('we are already loading this lil fella');
                        
                        return;
                    }
                    await scenario.study.load();
                }

                // get all of the objects we need to stream in
                const references = scenario.study.getAllReferences;
                console.log('refernces', references);
                // go through each one version ref to pull in
                for await (const versionRef of references) {
        
                    // we cover this by checking the view study if its loaded
                    // // load the version reference info
                    // if (!versionRef.hasLoaded) {
                    //     await versionRef.load();
                    // }

                    if (!versionRef.referenceObject) {
                        console.log('No reference loaded yet');
                        return;
                    }

                    const url = `https://sasaki.speckle.xyz/streams/${scenario.project.id}`;
                    const objUrl = `${url}/objects/${versionRef.referenceObject}`;
                    console.log(`getting object ${objUrl}`);

                    await viewer.current?.loadObjectAsync(
                        objUrl,
                        import.meta.env.VITE_SPECKLE_TOKEN
                    );
                }

                // the tree contains a root with a list of children nodes that represent each version the study pulls in
                const tree = viewer.current?.getDataTree();
                const cloudVersionRef = '823cb85551e5f8c4c1bd74c1f675e8ff';

                scenario.study.getCloudReferneces;
                viewer.current?.zoom([cloudVersionRef]);

                // TODO: lets adujst the camera to focus on the points
                // find the cloud and zoom in on it

                console.log('tree', tree);
            } catch (error) {
                console.error(error);
            }
        })();
    }, [scenario.study?.referenceKeys]);

    return <div className="Viewer" ref={viewerRef} />;
});
