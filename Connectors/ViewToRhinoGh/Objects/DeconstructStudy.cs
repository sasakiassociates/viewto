using System;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using ViewObjects;
using ViewTo.RhinoGh.Goo;

namespace ViewTo.RhinoGh.Objects
{
	public class DeconstructStudy : ViewToComponentBase
	{
		(int Clouds, int Targets, int Existing, int Proposals, int ResultClouds, int Viewers, int Options) _output;

		public DeconstructStudy() :
			base("Deconstruct Study", "DS", "Deconstruct a View Study", ConnectorInfo.Nodes.STUDY)
		{ }

		public override Guid ComponentGuid => new Guid("2A7AB720-A0B7-438E-9900-28B27119EEA7");

		protected override void RegisterInputParams(GH_InputParamManager pManager)
		{
			pManager.AddGenericParameter("View Study", "S", "View Study to deconstruct", GH_ParamAccess.item);
		}

		protected override void RegisterOutputParams(GH_OutputParamManager pManager)
		{
			var index = 0;
			pManager.AddParameter(new ViewObjParam("Clouds", "C", "View Clouds for a View Study", GH_ParamAccess.list));
			_output.Clouds = index++;
			pManager.AddParameter(new ViewObjParam("Results", "R", "Result Clouds for a View Study", GH_ParamAccess.list));
			_output.ResultClouds = index++;
			pManager.AddParameter(new ViewObjParam("Targets", "TC", "Target type Content for a View Study", GH_ParamAccess.list));
			_output.Targets = index++;
			pManager.AddParameter(new ViewObjParam("Existing", "EC", "Existing type Content for a View Study", GH_ParamAccess.list));
			_output.Existing = index++;
			pManager.AddParameter(new ViewObjParam("Propoals", "P", "Proposal type Content for a View Study", GH_ParamAccess.list));
			_output.Proposals = index++;
			pManager.AddParameter(new ViewObjParam("Options", "O", "Content Options for a View Study", GH_ParamAccess.list));
			_output.Options = index++;
			pManager.AddParameter(new ViewObjParam("Viewers", "V", "Viewers in a View Study", GH_ParamAccess.list));
			_output.Viewers = index;
		}

		protected override void SolveInstance(IGH_DataAccess DA)
		{
			GH_ObjectWrapper wrapper = null;
			DA.GetData(0, ref wrapper);

			if (wrapper?.Value is ViewStudy obj)
			{
				// TODO: Deal with the different types of view object types
				DA.SetDataList(_output.Clouds, obj.FindObjects<ViewCloudReference>());
				DA.SetDataList(_output.ResultClouds, obj.FindObjects<ResultCloud>());

				DA.SetDataList(_output.Targets, obj.FindObjects<ContentReference>().Where(x => x.ContentType == ContentType.Target));
				DA.SetDataList(_output.Existing, obj.FindObjects<ContentReference>().Where(x => x.ContentType == ContentType.Existing));
				DA.SetDataList(_output.Proposals, obj.FindObjects<ContentReference>().Where(x => x.ContentType == ContentType.Proposed));

				DA.SetDataList(_output.Viewers, obj.FindObjects<Viewer>());
				DA.SetDataList(_output.Options, obj.GetAllTargetContentInfo());
			}
		}
	}
}