using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ViewObjects;
using ViewTo.RhinoGh.Goo;

namespace ViewTo.RhinoGh.Setup
{
	public class CreateViewCloud : ViewToComponentBase
	{
		public CreateViewCloud() : base(
			"Create View Cloud",
			"CVC",
			"Cast a list of points to a view cloud",
			ConnectorInfo.Nodes.CLOUD)
		{ }

		// protected override Bitmap Icon => new Bitmap(Icons.GeneratePointsCloud);

		public override Guid ComponentGuid
		{
			get => new Guid("E4E4F6ED-2071-4F2E-9F9B-F687180143BF");
		}

		protected override void RegisterInputParams(GH_InputParamManager pManager)
		{
			pManager.AddTextParameter("Reference", "R", "Reference to a commit with a Point Cloud", GH_ParamAccess.item);
		}

		protected override void RegisterOutputParams(GH_OutputParamManager pManager)
		{
			pManager.AddParameter(new ViewObjParam("Cloud", "C", "View Cloud that references to a commit", GH_ParamAccess.item));
		}

		protected override void SolveInstance(IGH_DataAccess DA)
		{
			var reference = string.Empty;
			DA.GetData(0, ref reference);
			DA.SetData(0, new ViewCloudReference(new List<string> { reference }, ObjUtils.InitGuid));
		}
	}
}