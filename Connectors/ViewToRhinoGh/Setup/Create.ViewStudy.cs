using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using ViewObjects;
using ViewTo.RhinoGh.Goo;
using ViewTo.RhinoGh.Properties;
namespace ViewTo.RhinoGh.Setup
{

  public class CreateStudy : ViewToComponentBase
  {

    private (int Name, int Cloud, int Content, int Params) _input;

    public CreateStudy() : base(
      "Create View Study",
      "CS",
      "Create a View Study for a View Project",
      ConnectorInfo.Nodes.STUDY)
    {
    }

    public override Guid ComponentGuid => new Guid("328e44a9-91ba-450d-a40c-9da3bb7e0afc");

    protected override Bitmap Icon => new Bitmap(Icons.CreateViewStudy);

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      var i = 0;
      pManager.AddTextParameter("Name", "N", "Name of Study", GH_ParamAccess.item);
      _input.Name = i++;
      pManager.AddGenericParameter("Clouds", "C", "View Clouds for Study", GH_ParamAccess.list);
      _input.Cloud = i++;
      pManager.AddGenericParameter("Contents", "V", "Bundle of View Content for a study to use", GH_ParamAccess.list);
      _input.Content = i++;
      pManager.AddGenericParameter("Layouts", "L", "Viewer Bundles for Study", GH_ParamAccess.list);
      _input.Params = i;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new ViewObjParam("Study", "S", "View Study created and outputted as ViewObj Object", GH_ParamAccess.item));
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      var wrappers = new List<GH_ViewObj>();
      DA.GetDataList(_input.Cloud, wrappers);
      var clouds = wrappers.Unwrap<IViewObject>();

      wrappers.Clear();
      DA.GetDataList(_input.Content, wrappers);
      var contents = wrappers.Unwrap<IViewObject>();

      wrappers.Clear();
      DA.GetDataList(_input.Params, wrappers);
      var systems = wrappers.Unwrap<IViewObject>();

      var ghName = new GH_String();
      DA.GetData(_input.Name, ref ghName);

      var objs = new List<IViewObject>();

      objs.AddRange(contents);
      objs.AddRange(clouds);
      objs.AddRange(systems);

      var viewObj = new ViewStudy(objs, ghName.Value);

      DA.SetData(0, viewObj);
    }
  }
}
