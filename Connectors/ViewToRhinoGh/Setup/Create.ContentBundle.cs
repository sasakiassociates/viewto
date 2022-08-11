using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using ViewObjects.Content;
using ViewTo.RhinoGh.Goo;
using ViewTo.RhinoGh.Properties;

namespace ViewTo.RhinoGh.Setup
{
  public class CreateContentBundle : GH_Component
  {

    public CreateContentBundle() : base(
      "Create Content Bundle", "CVB", "Bundle up all the view content for a View Study", ConnectorInfo.CATEGORY, ConnectorInfo.Nodes.CONTENT)
    { }

    public override Guid ComponentGuid => new Guid("19AA924E-E52C-455F-9D6F-3CFBDEA0C9CE");
    
    protected override Bitmap Icon => new Bitmap(Icons.CreateContentBundle);

    private int _iTargets, _iBlockers, _iDesigns;

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      var i = 0;
      pManager.AddGenericParameter("Targets", "VT", "View Content Targets for Study", GH_ParamAccess.list);
      _iTargets = i++;
      pManager.AddGenericParameter("Blockers", "VB", "View Content Blockers for Study", GH_ParamAccess.list);
      _iBlockers = i++;
      pManager.AddGenericParameter("Designs", "VD", "View Content Designs for Study", GH_ParamAccess.list);
      _iDesigns = i;

      pManager[_iDesigns].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new ViewObjParam("ViewObj", "@obj", "View Obj as ViewObj Parameter Object", GH_ParamAccess.item));
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {

      var bundle = new ContentBundle();

      var wrappers = new List<GH_ViewObj>();
      DA.GetDataList(_iTargets, wrappers);
      bundle.contents.AddRange(wrappers.Unwrap<TargetContent>());

      wrappers = new List<GH_ViewObj>();
      DA.GetDataList(_iBlockers, wrappers);
      bundle.contents.AddRange(wrappers.Unwrap<BlockerContent>());

      wrappers = new List<GH_ViewObj>();
      DA.GetDataList(_iDesigns, wrappers);
      bundle.contents.AddRange(wrappers.Unwrap<DesignContent>());

      DA.SetData(0, bundle);
    }
  }
}
