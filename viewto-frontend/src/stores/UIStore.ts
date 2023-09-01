import { Store } from '@strategies/stores';
import { action, makeObservable, observable } from 'mobx';


export enum UIMode  {
    EXPLORE = 'Explore Mode',
    COMPARE = 'Compare Mode',
};

export enum Chart {
    VIEWY = 'Viewy',
    AREA = 'Area',
}


export default class UIStore extends Store {

    constructor() {
        super();
        makeObservable(this);
    }

    @observable
    appRef?: HTMLDivElement;

    @action
    setAppRef(appRef: HTMLDivElement) {
        this.appRef = appRef;
    }

    @observable
    chart: Chart = Chart.VIEWY;

    @action
    setChart(chart: Chart) {
        this.chart = chart;
    }

    @observable
    mode: UIMode = UIMode.EXPLORE;

    @action
    setMode(mode: UIMode) {
        this.mode = mode;
    }

    @observable
    camera: any = {};

    @action
    setCamera(camera: any) {
        this.camera = camera;
    }
    
    @observable
    startupModalIsOpen: boolean = true;

    @action
    setStartupModalOpen(isOpen = true) {
        this.startupModalIsOpen = isOpen;
    }
    
}
