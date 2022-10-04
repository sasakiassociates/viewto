using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using ViewObjects;
using ViewObjects.References;
using ViewObjects.Viewer;
using ViewTo.RhinoGh.Goo;

namespace ViewTo.RhinoGh.Setup
{

	public class CreateViewerLayout : ViewToComponentBase
	{
		public CreateViewerLayout() : base(
			"Create a Viewer Layout",
			"CVL",
			"Simple node for setting up different parameters for building a Viewer Rig",
			ConnectorInfo.Nodes.VIEWER)
		{ }

		public override Guid ComponentGuid
		{
			get => new Guid("1A51EF3A-A5CB-4F58-B509-B98203003861");
		}

		// protected override Bitmap Icon => new Bitmap(Icons.CreateViewerLayout);

		protected override void RegisterInputParams(GH_InputParamManager pManager)
		{
			pManager.AddGenericParameter("ViewClouds", "C", "View Clouds for Study", GH_ParamAccess.list);
			pManager[0].Optional = true;
		}

		protected override void RegisterOutputParams(GH_OutputParamManager pManager)
		{
			pManager.AddParameter(new ViewObjParam("ViewObj", "V", "View Obj as ViewObj Parameter Object", GH_ParamAccess.item));
		}

		protected override void SolveInstance(IGH_DataAccess DA)
		{
			var wrappers = new List<GH_ViewObj>();
			DA.GetDataList(0, wrappers);
			var clouds = wrappers.Unwrap<CloudReference>();

			var layout = new ViewerLayout(new List<ViewDirection>
			{
				ViewDirection.Front,
				ViewDirection.Right,
				ViewDirection.Back,
				ViewDirection.Left,
				ViewDirection.Up,
				ViewDirection.Down
			});

			if (!clouds.Any())
			{
				DA.SetData(0, new Viewer(new List<IViewerLayout> { layout }));
			}
			else
			{
				DA.SetData(0, new ViewerLinked(new List<IViewerLayout> { layout }, clouds.Where(x => x != null).Select(x => x.ViewId).ToList()));
			}
		}
	}
}