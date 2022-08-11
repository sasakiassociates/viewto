using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using ViewObjects.Content;
using ViewTo.RhinoGh.Goo;
using ViewTo.RhinoGh.Properties;
using Pipe = ViewTo.RhinoGh.ConnectorPipe;
namespace ViewTo.RhinoGh.Setup
{

  public class CreateViewContentProposed : GH_Component
  {

    public CreateViewContentProposed() : base(
      "Create View Proposed",
      "VCP",
      "Create Proposed Desgin Content for a View Study",
      ConnectorInfo.CATEGORY,
      ConnectorInfo.Nodes.CONTENT)
    { }

    public override Guid ComponentGuid => new Guid("b6cd7e15-8867-4269-9869-6245f32b62ea");
    
    protected override Bitmap Icon => new Bitmap(Icons.CreateContentProposed);

    private int _iName, _iData;
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      var index = 0;
      pManager.AddTextParameter("Name", "N", "Name of Content", GH_ParamAccess.item);
      _iName = index++;
      pManager.AddGenericParameter("Data", "D", "Geometry Data to use for View Content", GH_ParamAccess.list);
      _iData = index;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new ViewObjParam("ViewObj", "@obj", "View Obj as ViewObj Parameter Object", GH_ParamAccess.item));
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GH_String tempName = null;
      DA.GetData(_iName, ref tempName);

      var items = new List<GH_Mesh>();
      DA.GetDataList(_iData, items);

      var content = Pipe.Prime<DesignContent>(items, tempName);
    
      DA.SetData(0, content);
    }
  }
}
