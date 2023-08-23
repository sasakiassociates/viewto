import { FocusContext, ObstructingContext } from './Context';
import { Point } from './Point';
import { ViewCloud } from './ViewCloud';
import { ViewCondition, ConditionTypeLookUp } from './ViewCondition';
import { ViewResult } from './ViewResult';
import { ResultCloud } from './ResultCloud';

import { computed, action, makeObservable, observable } from 'mobx';

type easyName = {
    speckle_type: string;
};

export class ViewObjectTypes {
    static cloud: easyName = { speckle_type: 'ViewObjects.Speckle.ViewCloudReference' };
    static study: easyName = { speckle_type: 'ViewObjects.Speckle.ViewStudy' };
    static context: easyName = { speckle_type: 'ViewObjects.Speckle.ContentReference' };
    static result: easyName = { speckle_type: 'ViewObjects.Speckle.ResultCloud' };
}

export class ViewStudy {
    id: string;
    sasakiId: string;
    name: string;
    focuses: FocusContext[];
    obstructors: ObstructingContext[];
    clouds: ViewCloud[];
    results: ResultCloud[];

    constructor(data?: any) {
        makeObservable(this);
        this.id = data.id;
        this.name = data.ViewName;
        this.sasakiId = data.ViewId;
        let hackyIndex = 0;
        this.focuses = data.objects
            .filter(
                (x: any) =>
                    x.speckle_type == ViewObjectTypes.context.speckle_type &&
                    x.Content_Type == 'Potential'
            )
            .map((x: any) => this._focusContextToWeb(x));
        this.obstructors = data.objects
            .filter(
                (x: any) =>
                    x.speckle_type == ViewObjectTypes.context.speckle_type &&
                    x.Content_Type != 'Potential'
            )
            .map((x: any) => this._obstructorContextToWeb(x, hackyIndex++));
        this.clouds = data.objects
            .filter((x: any) => x.speckle_type == ViewObjectTypes.cloud.speckle_type)
            .map((x: any) => this._viewCloudToWeb(x));
        this.results = data.objects
            .filter((x: any) => x.speckle_type == ViewObjectTypes.result.speckle_type)
            .map((x: any) => this._resultCloudToWeb(x));
    }

    @observable
    points: Point[] = [];

    @computed
    get getContextReferences() {
        return [...this.focuses, ...this.obstructors]
            .map(version => version.references)
            .reduce((a, b) => [...a, ...b]);
    }

    @computed
    get getAllReferences() {
        return [...this.focuses, ...this.obstructors, ...this.clouds]
            .map(version => version.references)
            .reduce((a, b) => [...a, ...b]);
    }

    @computed
    get getCloudReferneces() {
        return [...this.clouds].map(version => version.references).reduce((a, b) => [...a, ...b]);
    }


    @computed
    get hasLoaded() {
        return this.getAllReferences.filter(ref => !ref.hasLoaded).length == 0;
    }

    @computed
    get isLoading() {
        return this.getAllReferences.filter(ref => !ref.isLoading).length == 0;
    }


    // conversions for getting Focus Context from speckle to app
    _focusContextToWeb(obj: any): FocusContext {
        return new FocusContext(obj.id, obj.ViewName, obj.ViewId, obj.References);
    }

    // conversions for getting Obstructors from speckle to app
    _obstructorContextToWeb(obj: any, index: number): ObstructingContext {
        const typeName = this._isProposed(obj) ? 'Proposed' : 'Existing';
        const name = obj.ViewName != null ? obj.ViewName : `${typeName} ${index}`;
        const item = new ObstructingContext(
            obj.id,
            name,
            obj.ViewId,
            obj.References,
            this._isProposed(obj)
        );
        console.log(item);
        return item;
    }

    // conversions for getting View Cloud from speckle to app
    _viewCloudToWeb(obj: any): ViewCloud {
        return new ViewCloud(obj.id, obj.References);
    }

    // conversions for getting all result cloud data
    _resultCloudToWeb(obj: any): ResultCloud {
        return new ResultCloud(
            obj.id,
            obj.ViewId,
            obj.Positions,
            obj.Data.map((x: any) => this._viewResultToWeb(x))
        );
    }

    // conversions for result cloud data structure
    _viewResultToWeb(obj: any) {
        return new ViewResult(
            obj.values,
            this._viewConditionToWeb(obj.Target_Id, obj.Content_Id, obj.ViewContentType)
        );
    }

    // conversions for getting View Condition from speckle to app
    _viewConditionToWeb(focus: string, obstructor: string, type: string): ViewCondition {
        return new ViewCondition(focus, obstructor, ConditionTypeLookUp[type.toLowerCase()]);
    }

    _isProposed(obj: any): boolean {
        return obj?.Content_Type == 'Proposed';
    }

    public async load() {
        console.log(`loading references: study ${this.name}`);

        // go through each one version ref to pull in
        for await (const versionRef of this.getAllReferences) {
            if (versionRef.hasLoaded) return;

            await versionRef.load();
        }
    }
}
