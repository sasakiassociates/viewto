import ObjectLoader, { SpeckleObject } from "@speckle/objectloader";

export function ObjectConverter() {

    // this needs to be fetched in the authentaticaiton process
    const token = "089149f2176eb6b5ba309f011ca24bcfc32376998e";
    // this should come from the login process
    const url = "https://speckle.xyz";
    // this comes from user input in the project panel
    const project = "a823053e07";
    // the result of project, model, version selection
    const study = "f4b16ebe7e93ea3ec653cd284d72ca05";

    const loader = new ObjectLoader({
        token: token,
        serverUrl: url,
        streamId: project,
        objectId: study,
        // options: {
        //     fullyTraverseArrays: false, // Default: false. By default, if an array starts with a primitive type, it will not be traversed. Set it to true if you want to capture scenarios in which lists can have intersped objects and primitives, e.g. [ 1, 2, "a", { important object } ]
        //     excludeProps: ["displayValue", "displayMesh", "__closure"], // Default: []. Any prop names that you pass in here will be ignored from object construction traversal.
        // },
    });


    const getObjectAndManuallyConvert = async (): Promise<SpeckleObject | SpeckleObject[]> => {
        const referenceObj = await loader.getAndConstructObject((e) => {
            // console.log("Progress ", e.stage, ":", e.current / e.total);
        });

        // deconstructing the speckle object
        // @ts-ignore
        const viewStudy = referenceObj.Data;
        console.log(viewStudy.ViewName + "{" + viewStudy.objects.length + "}\n");
        console.log(viewStudy);

        // grabbing the view clouds
        const cloud = viewStudy.objects.filter(x => x.speckle_type == "ViewObjects.Speckle.ViewCloudReference");
        console.log("cloud count=", cloud.length);

        // grabbing the context items for geometry
        const context = viewStudy.objects.filter(x => x.speckle_type == "ViewObjects.Speckle.ContentReference");
        console.log("context count=", context.length);

        // filtering out the different context within the study
        const targets = context.filter(x => x.Content_Type === "Potential");
        const existing = context.filter(x => x.Content_Type === "Existing");
        const proposed = context.filter(x => x.Content_Type === "Proposed");

        console.log("targets=", targets.length);
        console.log("existing=", existing.length);
        console.log("proposed=", proposed.length);

        /* 
        the result cloud contains all of the data we need to do the analysis exploration
        we just end up using the context pieces from the view study to load the geometry  
         */

        // grabbing the result cloud
        const result = viewStudy.objects.filter(x => x.speckle_type === "ViewObjects.Speckle.ResultCloud");
        console.log("result count=", result.length);

        const resultCloud = result[0];
        console.log("Result Cloud: ", resultCloud);

        // grabbing the point data from the cloud
        const points = resultCloud.Positions;
        console.log('point count', points.length / 3);

        // grabbing the view data from the cloud
        const viewData = resultCloud.Data;
        console.log('view data count:', viewData.length)

        // reading through the view conditions of the data
        viewData.map(x => {
            console.log(
                "View Data Condition\n" +
                "Type:" + x.ViewContentType + "\n" +
                "Target:" + x.Target_Name + " | " + x.Target_Id + "\n" +
                "Obstructor:" + x.Content_Name + " | " + x.Content_Id + "\n"
            );
        })


        return referenceObj;
    };

    getObjectAndManuallyConvert();
}