using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using ViewObjects;
using ViewObjects.Clouds;
using ViewObjects.Studies;

namespace ViewTo.RhinoGh.Setup
{

  public class DeconstructContentOption : ViewToComponentBase
  {

    private(int targetName, int targetId, int contentName, int contentId, int stage) _out;

    public DeconstructContentOption() : base("Deconstruct Content Option", "DC", "Break up a content option to know what target, content, and stage it referes to",
      ConnectorInfo.Nodes.RESULTS)
    { }

    public override Guid ComponentGuid => new Guid("F65839D4-3A09-4BA9-A017-595BB522E408");

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("View Object", "VO", $"A {nameof(ViewStudy)} or {nameof(ResultCloud)}", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      var index = 0;
      pManager.AddTextParameter("Target Name", "TN", "Name of the target content used", GH_ParamAccess.list);
      _out.targetName = index++;
      pManager.AddTextParameter("Target ID", "TI", "ID of the target content used", GH_ParamAccess.list);
      _out.targetId = index++;
      pManager.AddTextParameter("Content Name", "CN", "Name of the blocking content used", GH_ParamAccess.list);
      _out.contentName = index++;
      pManager.AddTextParameter("Content ID", "CI", "ID of the blocking content used", GH_ParamAccess.list);
      _out.contentId = index++;
      pManager.AddTextParameter("Stage", "S", "The stage used during this option", GH_ParamAccess.list);
      _out.stage = index;

    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {

      GH_ObjectWrapper wrapper = null;
      DA.GetData(0, ref wrapper);

      if(wrapper == null)
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No Valid View Study Found");

        return;
      }

      if(wrapper.Value is ViewStudy vs)
      {
        var resultCloud = vs.Get<ResultCloud>();

        if(resultCloud == null)
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No Result Cloud Found in Study");
          return;
        }

        List<string> targetNames = new List<string>();
        List<string> targetIds = new List<string>();
        List<string> contentNames = new List<string>();
        List<string> contentIds = new List<string>();
        List<string> stages = new List<string>();

        foreach(var opt in resultCloud.GetAllOpts())
        {
          targetNames.Add(opt.target.name);
          targetIds.Add(opt.target.guid);
          contentNames.Add(opt.content.name);
          contentIds.Add(opt.content.guid);
          stages.Add($"{opt.stage}");
        }

        DA.SetDataList(_out.targetName, targetNames);
        DA.SetDataList(_out.targetId, targetIds);
        DA.SetDataList(_out.contentName, contentNames);
        DA.SetDataList(_out.contentId, contentIds);
        DA.SetDataList(_out.stage, stages);

      }

    }
  }

}
