using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using ViewObjects;
using ViewObjects.Cloud;
using ViewObjects.Content;
using ViewObjects.Study;
using ViewObjects.Viewer;
using ViewTo.RhinoGh.Goo;
using ViewTo.RhinoGh.Properties;

namespace ViewTo.RhinoGh.Setup
{

	public class CreateStudy : ViewToComponentBase
	{
		public CreateStudy() : base(
			"Create View Study",
			"CS",
			"Create a View Study for a View Project",
			ConnectorInfo.Nodes.STUDY)
		{ }

		public override Guid ComponentGuid => new Guid("328e44a9-91ba-450d-a40c-9da3bb7e0afc");

		protected override Bitmap Icon => new Bitmap(Icons.CreateViewStudy);

		(int Name, int Cloud, int Content, int Params) _input;

		protected override void RegisterInputParams(GH_InputParamManager pManager)
		{
			var i = 0;
			pManager.AddTextParameter("Name", "N", "Name of Study", GH_ParamAccess.item);
			_input.Name = i++;
			pManager.AddGenericParameter("ViewClouds", "C", "View Clouds for Study", GH_ParamAccess.list);
			_input.Cloud = i++;
			pManager.AddGenericParameter("ViewContent Bundle", "B", "Bundle of View Content for a study to use", GH_ParamAccess.list);
			_input.Content = i++;
			pManager.AddGenericParameter("Viewer Bundle", "V", "Viewer Bundles for Study", GH_ParamAccess.list);
			_input.Params = i;
		}

		protected override void RegisterOutputParams(GH_OutputParamManager pManager)
		{
			pManager.AddParameter(new ViewObjParam("ViewObj", "V", "View Obj as ViewObj Parameter Object", GH_ParamAccess.item));
		}

		protected override void SolveInstance(IGH_DataAccess DA)
		{
			var wrappers = new List<GH_ViewObj>();
			DA.GetDataList(_input.Cloud, wrappers);
			var clouds = wrappers.Unwrap<ViewCloud>();

			wrappers.Clear();
			DA.GetDataList(_input.Content, wrappers);
			var contentBundle = wrappers.Unwrap<ContentBundle>();

			wrappers.Clear();
			DA.GetDataList(_input.Params, wrappers);
			var viewerBundles = wrappers.Unwrap<ViewerBundle>();

			var res = new GH_String();

			DA.GetData(_input.Name, ref res);
			var viewObj = new ViewStudy
			{
				viewName = res.Value,
				objs = new List<IViewObj>()
			};

			viewObj.objs.AddRange(contentBundle);
			viewObj.objs.AddRange(clouds);
			viewObj.objs.AddRange(viewerBundles);

			DA.SetData(0, viewObj);
		}

	}
}