using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using ViewObjects.Clouds;
using ViewObjects.Contents;
using ViewObjects.Results;
using ViewObjects.Systems.Layouts;
using ViewTo.RhinoGh.Goo;

namespace ViewTo.RhinoGh.Setup
{

  public class CreateResultCloudComponent : ViewToComponentBase
  {

    private(int Points, int Id, int Target, int Values) _input;

    public CreateResultCloudComponent() : base(
      "Create Result Cloud",
      "CRC",
      "Create a cloud with view analysis data stored in it",
      ConnectorInfo.Nodes.RESULTS)
    { }

    public override Guid ComponentGuid => new Guid("BEC145EE-4CD5-4582-95C6-A6214A3786DA");

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      var index = 0;

      pManager.AddPointParameter("Points", "P", "Points for Cloud", GH_ParamAccess.list);
      _input.Points = index++;

      pManager.AddTextParameter("ID", "I", "Id for the View Cloud", GH_ParamAccess.item);
      _input.Id = index++;

      pManager.AddIntegerParameter("Values", "V", "Values to use", GH_ParamAccess.tree);
      _input.Values = index++;

      pManager.AddGenericParameter("Content", "C", "Content to use", GH_ParamAccess.tree);
      _input.Target = index;

      pManager[_input.Id].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new ViewObjParam("Result Cloud", "R", "Rescult Cloud", GH_ParamAccess.item));
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      var points = new List<GH_Point>();
      DA.GetDataList(_input.Points, points);

      DA.GetDataTree(_input.Values, out GH_Structure<GH_Integer> treeValues);

      DA.GetDataTree(_input.Target, out GH_Structure<GH_ObjectWrapper> treeOptions);

      var cloudPoints = (from t in points select new CloudPoint {x = t.Value.X, y = t.Value.Y, z = t.Value.Z}).ToArray();

      var id = string.Empty;

      if(DA.GetData(_input.Id, ref id) && Guid.TryParse(id, out var r))
      {
        id = r.ToString();
      }
      else
      {
        id = Guid.NewGuid().ToString();
      }

      var dataContainer = new List<IResultCloudData>();

      for(var bIndex = 0; bIndex < treeOptions.Branches.Count; bIndex++)
      {
        var branchValue = treeValues.Branches[bIndex];
        var values = new List<int>();

        foreach(var v in branchValue)
        {
          values.Add(v.Value);
        }

        if(treeOptions.Branches[bIndex].FirstOrDefault().Value is ContentOption co)
        {
          dataContainer.Add(new ResultCloudData
            {Values = values, Option = co, Layout = nameof(Layout)});
        }
      }

      var resulCloud = new ResultCloud(cloudPoints, dataContainer, id);
      DA.SetData(0, resulCloud);
    }
  }

}
