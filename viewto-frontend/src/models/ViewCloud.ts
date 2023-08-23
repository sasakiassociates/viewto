import ReferenceObject from "./ReferenceObject";

export class ViewCloud {
    id: string;
    references: ReferenceObject[];

    /**
     *
     */
    constructor(id: string, references: string[]) {
        this.id = id;
        this.references = references.map(ref => new ReferenceObject(ref));
    }
}

