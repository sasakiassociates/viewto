import {observable, action, computed, makeObservable} from 'mobx'

export class Point {
    id: number;
    x: number;
    y: number;
    z: number;

    /**
     *
     */
    constructor( id: number, x: number, y: number, z: number) {
        makeObservable(this);
        this.id = id;
        this.x = x;
        this.y = y;
        this.z = z;
    }


    @observable
    active : boolean = false;
    
    @action
    setActive(value: boolean){
        this.active = value;
    }   
 }

