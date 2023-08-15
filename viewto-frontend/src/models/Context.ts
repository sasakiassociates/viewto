// a simple reference object used to keep track of the speckle reference id and the sasaki app id  
export class Context {
    // name of the context object
    name: string;
    // id of this sasaki object
    sasakiId: string;
    // reference id to the speckle object
    references: string[];

    /**
     *
     */
    constructor(name: string, sasakiId: string, references: string[]) {
        this.name = name;
        this.sasakiId = sasakiId;
        this.references = references;
    }
}


// similar to the context class but with a specific type declared for view studies
export class FocusContext extends Context { }

// similar to the context class but can be a toggle for the proposed context type
export class ObstructingContext extends Context {
    // toggle for declaring what group the obstructor is 
    proposed: boolean = false;

    /**
     *
     */
    constructor(name: string, sasakiId: string, references: string[], proposed: boolean) {
        super(name, sasakiId, references);
        this.proposed = proposed;
    }

}


