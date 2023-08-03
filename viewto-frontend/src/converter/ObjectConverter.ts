import ObjectLoader, { SpeckleObject } from "@speckle/objectloader";

export function ObjectConverter() {

    const token = "089149f2176eb6b5ba309f011ca24bcfc32376998e";
    const url = "https://speckle.xyz";
    const project = "a823053e07";
    const study = "f4b16ebe7e93ea3ec653cd284d72ca05";
    const castle = "3c9c2f7937efefc8551696d618f9e671";

    const loader = new ObjectLoader({
        token: token,
        serverUrl: url,
        streamId: project,
        objectId: study,
        options: {
            fullyTraverseArrays: false, // Default: false. By default, if an array starts with a primitive type, it will not be traversed. Set it to true if you want to capture scenarios in which lists can have intersped objects and primitives, e.g. [ 1, 2, "a", { important object } ]
            excludeProps: ["displayValue", "displayMesh", "__closure"], // Default: []. Any prop names that you pass in here will be ignored from object construction traversal.
        },
    });

    const getData = async (): Promise<SpeckleObject | SpeckleObject[]> => {
        const data = await loader.getAndConstructObject((e) => {
            console.log("Progress ", e.stage, ":", e.current / e.total);
        });
        console.log(data);
        return data;
    };

    getData();
}