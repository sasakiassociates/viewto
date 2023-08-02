class view{

    name:string;
    point: number;
    camera : {};
    focuses :[];
    condition : {};
    pixelRange : [2];
    valueRange : [2];
    texture: string;

    // volatile 
    active: boolean;
}

export default view;