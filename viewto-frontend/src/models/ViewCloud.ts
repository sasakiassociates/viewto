
import { Model, model, prop } from "mobx-keystone";
import { ViewObjectTypes } from "./ViewStudy";

@model("viewto/ViewCloud")
export class ViewCloud extends Model({
    id: prop<string>(""),
    sasakiId: prop<string>(""),
    points: prop<number[]>(() => [])
}) {}
