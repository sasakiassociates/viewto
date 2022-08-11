using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using ViewObjects;
using ViewObjects.Cloud;
using ViewObjects.Viewer;
using ViewTo.RhinoGh.Goo;
using ViewTo.RhinoGh.Properties;

namespace ViewTo.RhinoGh.Setup
{

  public class CreateViewerLayout : GH_Component
  {
    public CreateViewerLayout() : base(
      "Create a Viewer Layout",
      "CVL",
      "Simple node for setting up different parameters for building a Viewer Rig",
      ConnectorInfo.CATEGORY,
      ConnectorInfo.Nodes.VIEWER)
    { }

    public override Guid ComponentGuid => new Guid("1A51EF3A-A5CB-4F58-B509-B98203003861");

    // protected override Bitmap Icon => new Bitmap(Icons.CreateViewerLayout);

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("ViewClouds", "C", "View Clouds for Study", GH_ParamAccess.list);
      pManager[0].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new ViewObjParam("ViewObj", "V", "View Obj as ViewObj Parameter Object", GH_ParamAccess.item));
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      var wrappers = new List<GH_ViewObj>();
      DA.GetDataList(0, wrappers);
      var clouds = wrappers.Unwrap<ViewCloud>();

      IViewerBundle vb;
      if (clouds != null && clouds.Any())
        vb = new ViewerBundleLinked
        {
          linkedClouds = clouds.Where(x => x != null).Select(x => x.Build()).ToList(),
          layouts = new List<IViewerLayout>
          {
            new ViewerLayoutHorizontal()
          }
        };
      else
        vb = new ViewerBundle
        {
          layouts = new List<IViewerLayout>
          {
            new ViewerLayoutHorizontal()
          }
        };


      DA.SetData(0, vb);
    }

  }
}
