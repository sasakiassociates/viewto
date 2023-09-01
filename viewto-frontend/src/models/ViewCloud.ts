import VersionReference from "./VersionReference";

export class ViewCloud {
    id: string;
    references: VersionReference[];

    /**
     *
     */
    constructor(id: string, references: string[]) {
        this.id = id;
        this.references = references.map(ref => new VersionReference(ref));
    }
}

