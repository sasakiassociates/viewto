using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ViewObjects;
using ViewObjects.Common;
using ViewObjects.References;
using ViewTo.RhinoGh.Goo;
using ViewTo.RhinoGh.Properties;

namespace ViewTo.RhinoGh.Setup
{

  public abstract class CreateViewContent : ViewToComponentBase
  {

    (int Name, int References) _input;

    public CreateViewContent(string name, string nickname, string description) : base(name, nickname, description, ConnectorInfo.Nodes.CONTENT)
    { }

    protected abstract ContentType ContentType { get; }

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      var index = 0;
      pManager.AddTextParameter("References", "R", "List of references to link the content to", GH_ParamAccess.list);
      _input.References = index++;
      pManager.AddTextParameter("Name", "N", "Name of Content", GH_ParamAccess.item);
      _input.Name = index;

      pManager[_input.Name].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new ViewObjParam("Content", "C", "Content for a View Study", GH_ParamAccess.item));
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GH_String tempName = null;
      DA.GetData(_input.Name, ref tempName);

      var items = new List<GH_String>();
      DA.GetDataList(_input.References, items);

      DA.SetData(0, new ContentReference(
        items.Where(x => x != null).Select(x => x.Value).ToList(),
        ContentType,
        ObjUtils.InitGuid,
        tempName?.Value)
      );
    }
  }

  public class CreateViewContentTarget : CreateViewContent
  {

    public CreateViewContentTarget() : base(
      "Create View Target",
      "CT",
      "Setup target content that will be analyzed in a view study")
    { }

    protected override ContentType ContentType => ContentType.Potential;

    protected override Bitmap Icon => new Bitmap(Icons.CreateContentTarget);

    public override Guid ComponentGuid => new Guid("b686629d-ccbb-4534-96f0-ca33550bbff7");
  }

  public class CreateViewContentProposed : CreateViewContent
  {

    public CreateViewContentProposed() : base(
      "Create View Proposed",
      "CP",
      "Create Proposed Desgin Content for a View Study")
    { }

    public override Guid ComponentGuid => new Guid("b6cd7e15-8867-4269-9869-6245f32b62ea");

    protected override Bitmap Icon => new Bitmap(Icons.CreateContentProposed);

    protected override ContentType ContentType => ContentType.Proposed;
  }

  public class CreateViewContentExisting : CreateViewContent
  {

    public CreateViewContentExisting() : base(
      "Create View Existing",
      "CE",
      "Setup Exisiting content for a view study")
    { }

    protected override Bitmap Icon => new Bitmap(Icons.CreateContentExisting);

    public override Guid ComponentGuid => new Guid("ca8eb77a-521e-4361-93c5-c44ff074e18f");

    protected override ContentType ContentType => ContentType.Existing;
  }

}
