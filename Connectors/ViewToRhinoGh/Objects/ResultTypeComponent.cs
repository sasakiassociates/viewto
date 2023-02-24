using GH_IO.Serialization;
using Grasshopper.Kernel;
using System;
using System.Windows.Forms;
using ViewObjects;

namespace ViewTo.RhinoGh.Objects
{

  public class ResultTypeComponent : ViewToComponentBase
  {

    private ViewContentType _viewContentType = ViewContentType.Proposed;

    public ResultTypeComponent() : base("Result Type",
      "RT",
      "A quick way of selecting a result type",
      ConnectorInfo.Nodes.RESULTS)
    { }

    public override Guid ComponentGuid => new Guid("4308AD20-1745-41CF-B567-395D15B1E62E");

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    { }

    public override bool Write(GH_IWriter writer)
    {
      writer.SetString("resultTypeName", _viewContentType.ToString());
      return base.Write(writer);
    }

    public override bool Read(GH_IReader reader)
    {
      var value = "Undefined";
      reader.TryGetString("resultTypeName", ref value);
      _viewContentType = (ViewContentType)Enum.Parse(typeof(ViewContentType), value);

      return base.Read(reader);
    }

    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
    {
      foreach(ViewContentType rt in Enum.GetValues(typeof(ViewContentType)))
      {
        Menu_AppendItem(
          menu,
          rt.ToString(), (s, e) =>
          {
            if(s is ToolStripMenuItem item && item.Tag is ViewContentType tag)
            {
              _viewContentType = tag;
              ExpireSolution(true);
            }
          },
          true,
          rt == _viewContentType
        ).Tag = rt;
      }

      base.AppendAdditionalComponentMenuItems(menu);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddTextParameter("Result Type", "R", "Name of Result Type", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      DA.SetData(0, _viewContentType.ToString());
    }
  }

}
