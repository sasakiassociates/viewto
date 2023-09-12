using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using Sasaki.Common;
using ViewObjects.Contents;

namespace ViewTo.RhinoGh.Setup
{

  public class CreateContentOption : ViewToComponentBase
  {

    private(int Names, int Ids) _input;

    public CreateContentOption() : base("Create View Content Option", "CCO", "Simple node for creating a view content option for exploring results",
      ConnectorInfo.Nodes.RESULTS)
    { }

    public override Guid ComponentGuid => new Guid("B9AE10FF-B47C-4F71-9A46-7288A5B16624");

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      var index = 0;
      pManager.AddTextParameter("Name", "N", "Name to use", GH_ParamAccess.list);
      _input.Names = index++;
      pManager.AddTextParameter("Id", "I", "Id to use", GH_ParamAccess.list);
      _input.Ids = index;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Options", "O", "List of View Content Options", GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      var names = new List<string>();
      DA.GetDataList(_input.Names, names);

      var ids = new List<string>();
      DA.GetDataList(_input.Ids, ids);

      var options = new List<ContextInfo>();

      if(ids.Valid() && names.Valid() && names.Count == ids.Count)
      {
        for(var i = 0; i < ids.Count; i++)
        {
          options.Add(new ContextInfo(ids[i], names[i]));
        }
      }

      if(!options.Valid())
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The content options are not valid");
      }

      DA.SetDataList(0, options);
    }
  }

}
