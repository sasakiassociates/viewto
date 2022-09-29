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
	public class CreateResultCloudComponent : ViewToComponentBase
	{

		(int Points, int Id, int Target, int Values) _input;

		public CreateResultCloudComponent() : base(
			"Create Result Cloud",
			"CRC",
			"Create a cloud with view analysis data stored in it",
			ConnectorInfo.Nodes.RESULTS)
		{ }

		public override Guid ComponentGuid
		{
			get => new Guid("BEC145EE-4CD5-4582-95C6-A6214A3786DA");
		}

		protected override void RegisterInputParams(GH_InputParamManager pManager)
		{
			var index = 0;

			pManager.AddPointParameter("Points", "P", "Points for Cloud", GH_ParamAccess.list);
			_input.Points = index++;

			pManager.AddTextParameter("ID", "I", "Id for the View Cloud", GH_ParamAccess.item);
			_input.Id = index++;

			pManager.AddIntegerParameter("Values", "V", "Values to use", GH_ParamAccess.tree);
			_input.Values = index++;

			pManager.AddTextParameter("Names", "T", "Target content to use", GH_ParamAccess.tree);
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

			DA.GetDataTree(_input.Target, out GH_Structure<GH_String> treeNames);

			var cloudPoints = (from t in points select new CloudPoint
			{
				x = t.Value.X, y = t.Value.Y, z = t.Value.Z
			}).ToArray();

			var id = string.Empty;

			if (DA.GetData(_input.Id, ref id) && Guid.TryParse(id, out var r))
				id = r.ToString();
			else
				id = Guid.NewGuid().ToString();

			var dataContainer = new List<IResultData>();

			const int INDEX_CONTENT = 0;
			const int INDEX_STAGE = 1;

			for (var bIndex = 0; bIndex < treeNames.Branches.Count; bIndex++)
			{
				var branchValue = treeValues.Branches[bIndex];

				var values = new List<int>();
				foreach (var v in branchValue)
					values.Add(v.Value);

				var branchName = treeNames.Branches[bIndex];
				dataContainer.Add(
					new ContentResultData(
						values,
						branchName.Count >= INDEX_CONTENT ? branchName[INDEX_CONTENT].Value : "Invalid Content",
						branchName.Count >= INDEX_STAGE ? branchName[INDEX_STAGE].Value : "Invalid Stage",
						0)
				);
			}

			var resulCloud = new ResultCloud
			{
				ViewId = id,
				points = cloudPoints,
				data = dataContainer
			};

			DA.SetData(0, resulCloud);
		}
	}
}