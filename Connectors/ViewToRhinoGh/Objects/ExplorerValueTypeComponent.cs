using GH_IO.Serialization;
using Grasshopper.Kernel;
using System;
using System.Windows.Forms;
using ViewObjects;
namespace ViewTo.RhinoGh.Objects
{
  public class ExplorerValueTypeComponent : ViewToComponentBase
  {

    private ExplorerValueType _type = ExplorerValueType.ExistingOverPotential;

    public ExplorerValueTypeComponent() : base("Explorer Value Type",
      "EV",
      "A set of options for comparing explorer values",
      ConnectorInfo.Nodes.RESULTS)
    {
    }

    public override Guid ComponentGuid => new Guid("3388A5A6-6969-4ED0-8155-22D94334DE30");

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
    }

    public override bool Write(GH_IWriter writer)
    {
      writer.SetString("type", _type.ToString());
      return base.Write(writer);
    }

    public override bool Read(GH_IReader reader)
    {
      var value = "ExistingOverPotential";
      reader.TryGetString("type", ref value);
      _type = (ExplorerValueType)Enum.Parse(typeof(ExplorerValueType), value);

      return base.Read(reader);
    }

    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
    {
      foreach (ExplorerValueType rt in Enum.GetValues(typeof(ExplorerValueType)))
      {
        Menu_AppendItem(
          menu,
          rt.ToString(), (s, e) =>
          {
            if (s is ToolStripMenuItem item && item.Tag is ExplorerValueType tag)
            {
              _type = tag;
              ExpireSolution(true);
            }
          },
          true,
          rt == _type
        ).Tag = rt;
      }

      base.AppendAdditionalComponentMenuItems(menu);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddTextParameter("Explorer Value Type", "V", "Type of Explorer Value Type", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      DA.SetData(0, _type.ToString());
    }
  }
}
