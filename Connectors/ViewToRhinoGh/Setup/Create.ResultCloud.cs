using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using ViewObjects;
using ViewObjects.Cloud;
using ViewObjects.Content;
using ViewTo.RhinoGh.Goo;

namespace ViewTo.RhinoGh.Setup
{
	public class CreateResultCloudComponent : GH_Component
	{

		public CreateResultCloudComponent() : base(
			"Create Result Cloud",
			"CRC",
			"Create a cloud with view analysis data stored in it",
			ConnectorInfo.CATEGORY,
			ConnectorInfo.Nodes.RESULTS)
		{ }

		(int Points, int Id, int Target, int Values, int Force) _input;

		protected override void RegisterInputParams(GH_InputParamManager pManager)
		{
			var index = 0;

			pManager.AddPointParameter("Points", "P", "Points for Cloud", GH_ParamAccess.list);
			_input.Points = index++;

			pManager.AddTextParameter("ID", "I", "Id for the View Cloud", GH_ParamAccess.item);
			_input.Id = index++;

			pManager.AddNumberParameter("Values", "V", "Values to use", GH_ParamAccess.tree);
			_input.Values = index++;

			pManager.AddTextParameter("Names", "T", "Target content to use", GH_ParamAccess.tree);
			_input.Target = index++;

			pManager.AddBooleanParameter("Force", "f", "Force the cloud to rebuild", GH_ParamAccess.item, false);
			_input.Force = index;

			pManager[_input.Id].Optional = true;
			pManager[_input.Force].Optional = true;
		}

		protected override void RegisterOutputParams(GH_OutputParamManager pManager)
		{
			pManager.AddParameter(new ViewObjParam("Result Cloud", "R", "Rescult Cloud", GH_ParamAccess.item));
		}

		string _storedId = string.Empty;

		protected override void SolveInstance(IGH_DataAccess DA)
		{
			var points = new List<GH_Point>();
			DA.GetDataList(_input.Points, points);

			DA.GetDataTree(_input.Values, out GH_Structure<GH_Number> treeValues);

			DA.GetDataTree(_input.Target, out GH_Structure<GH_String> treeNames);
			
			GH_Boolean force = new GH_Boolean();
			DA.GetData(_input.Force, ref force);

			var cloudPoints = (from t in points select new CloudPoint
			{
				x = t.Value.X, y = t.Value.Y, z = t.Value.Z
			}).ToArray();

			var id = string.Empty;
			if (DA.GetData(_input.Id, ref id) && Guid.TryParse(id, out var r))
				id = r.ToString();
			else
				id = Guid.NewGuid().ToString();

			if (_storedId.Valid() && _storedId.Equals(id) && !force.Value)
			{
				AddRuntimeMessage(GH_RuntimeMessageLevel.Blank, "Cloud is already populated");
				return;
			}

			_storedId = id;

			var dataContainer = new List<IResultData>();

			for (int bIndex = 0; bIndex < treeNames.Branches.Count; bIndex++)
			{
				var branchValue = treeValues.Branches[bIndex];

				var values = new List<double>();
				foreach (var v in branchValue)
					values.Add(v.Value);

				var branchName = treeNames.Branches[bIndex];
				dataContainer.Add(new ContentResultData(values, stage: branchName[1].Value, content: branchName[0].Value, 0));
			}

			var resulCloud = new ResultCloud
			{
				viewID = id,
				points = cloudPoints,
				data = dataContainer
			};

			DA.SetData(0, resulCloud);
		}

		public override Guid ComponentGuid => new Guid("BEC145EE-4CD5-4582-95C6-A6214A3786DA");
	}
}