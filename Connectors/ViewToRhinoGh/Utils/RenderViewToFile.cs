using Grasshopper.Kernel;
using Rhino;
using System;
using System.Collections.Generic;
using System.IO;

namespace ViewTo.RhinoGh.Results
{

  public class RenderViewToFile : ViewToComponentBase
  {

    ( int run, int path, int prefix, int views ) _input;

    public RenderViewToFile() : base(
      "Render View",
      "RV",
      "Render View to File",
      ConnectorInfo.Nodes.UTIL)
    { }

    public override Guid ComponentGuid => new Guid("604853A2-05CF-4C70-8ABE-479221B53BA0");

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      var index = 0;

      pManager.AddTextParameter("File Path", "F", "File path to render the images to", GH_ParamAccess.item);
      _input.path = index++;

      pManager.AddTextParameter("Prefix", "P", "File name prefix to use with each image", GH_ParamAccess.item);
      _input.prefix = index++;

      pManager.AddTextParameter("ViewPorts", "V", "Name of which view ports to render with ", GH_ParamAccess.list);
      _input.views = index++;

      pManager.AddBooleanParameter("Run", "R", "Toggle and run through points", GH_ParamAccess.item);
      _input.run = index;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddTextParameter("Files", "F", "File paths of image saved", GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      var run = false;
      DA.GetData(_input.run, ref run);
      var dir = string.Empty;
      DA.GetData(_input.path, ref dir);
      var prefix = string.Empty;
      DA.GetData(_input.prefix, ref dir);
      var vps = new List<string>();
      DA.GetDataList(_input.views, vps);

      if(!run)
      {
        return;
      }

      if(string.IsNullOrWhiteSpace(prefix))
      {
        return;
      }

      if(string.IsNullOrWhiteSpace(dir))
      {
        return;
      }

      if(!dir.EndsWith(Path.DirectorySeparatorChar.ToString()))
      {
        dir += Path.DirectorySeparatorChar;
      }

      if(!Directory.Exists(dir))
      {
        Directory.CreateDirectory(dir);
      }

      var files = new List<string>();

      foreach(var vp in vps)
      {
        var view = RhinoDoc.ActiveDoc.Views.Find(vp, false);
        if(view == null)
        {
          continue;
        }

        var fileName = Path.Combine(dir, vp, prefix);

        var image = view.CaptureToBitmap(false, false, false);
        image.Save(fileName);
        image.Dispose();

        files.Add(fileName);
      }

      DA.SetDataList(0, files);
    }
  }

}
