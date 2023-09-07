import VersionReference from './VersionReference';
// a simple reference object used to keep track of the speckle reference id and the sasaki app id

export class Context implements Referencer, Info {
    // name of the context object
    name: string;
    // the speckle id
    id: string;
    // id of this sasaki object
    sasakiId: string;
    // reference id to the speckle object
    references: VersionReference[];

    /**
     *
     */
    constructor(id: string, name: string, sasakiId: string, references: string[]) {
        this.id = id;
        this.name = name;
        this.sasakiId = sasakiId;
        this.references = references.map(ref => new VersionReference(ref));
    }

    
}

// similar to the context class but with a specific type declared for view studies
export class FocusContext extends Context {}

// similar to the context class but can be a toggle for the proposed context type
export class ObstructContext extends Context {
    // toggle for declaring what group the obstructor is
    proposed: boolean = false;

    /**
     *
     */
    constructor(
        id: string,
        name: string,
        sasakiId: string,
        references: string[],
        proposed: boolean
    ) {
        super(id, name, sasakiId, references);
        this.proposed = proposed;
    }
}

export interface Info {
    // name of the context object
    name: string;
    // the speckle id
    id: string;
}

export interface Referencer {
    // reference id to the speckle object
    references: VersionReference[];
}
