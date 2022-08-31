using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using ViewTo.RhinoGh.Goo;
using Pipe = ViewTo.RhinoGh.ConnectorPipe;

namespace ViewTo.RhinoGh.Setup
{
	public class CreateViewCloud : ViewToComponentBase
	{
		public CreateViewCloud() : base(
			"Create View Cloud", "CVC",
			"Cast a list of points to a view cloud",
			ConnectorInfo.Nodes.CLOUD)
		{ }

		// protected override Bitmap Icon => new Bitmap(Icons.GeneratePointsCloud);

		public override Guid ComponentGuid => new Guid("E4E4F6ED-2071-4F2E-9F9B-F687180143BF");

		protected override void RegisterInputParams(GH_InputParamManager pManager)
		{
			pManager.AddPointParameter("Points", "P", "Points to Convert to Cloud", GH_ParamAccess.list);
		}

		protected override void RegisterOutputParams(GH_OutputParamManager pManager)
		{
			pManager.AddParameter(new ViewObjParam("ViewCloud", "viewobj", "View Obj as ViewObj Parameter Object", GH_ParamAccess.item));
		}

		protected override void SolveInstance(IGH_DataAccess DA)
		{
			var points = new List<GH_Point>();
			DA.GetDataList(0, points);

			var viewObj = Pipe.Prime(points);
			DA.SetData(0, viewObj);
		}
	}
}