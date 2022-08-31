using System;
using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;

namespace ViewTo.RhinoGh.Results
{
	public class MoveToView : ViewToComponentBase
	{
		public MoveToView(	) :
			base("Move To View",
			     "MV",
			     "Move the rhino viewport to specific point and target",
			     ConnectorInfo.Nodes.UTIL)
		{ }

		(int camera, int target, int horizontal, int vertical) _input;

		protected override void RegisterInputParams(GH_InputParamManager pManager)
		{
			var index = 0;

			pManager.AddPointParameter("Camera Point", "C", "The location for the camera to be positioned at", GH_ParamAccess.item);
			_input.camera = index++;

			pManager.AddPointParameter("Target Point", "T", "The location for the camera target to be positioned at", GH_ParamAccess.item);
			_input.target = index++;

			pManager.AddNumberParameter("Horizontal Angle", "H", "Set the Horizontal rotation of the camera", GH_ParamAccess.item, 0.0);
			_input.horizontal = index++;

			pManager.AddNumberParameter("Vertical Angle", "V", "Set the Vertical rotation of the camera", GH_ParamAccess.item, 0.0);
			_input.vertical = index;

			pManager[_input.horizontal].Optional = true;
			pManager[_input.vertical].Optional = true;
		}

		protected override void RegisterOutputParams(GH_OutputParamManager pManager)
		{ }

		protected override void SolveInstance(IGH_DataAccess DA)
		{
			Point3d cameraPoint = default;
			DA.GetData(_input.camera, ref cameraPoint);

			Point3d targetPoint = default;
			DA.GetData(_input.target, ref targetPoint);

			var horizontalAngle = 0.0;
			DA.GetData(_input.horizontal, ref horizontalAngle);

			var verticalAngle = 0.0;
			DA.GetData(_input.vertical, ref verticalAngle);

			var view = RhinoDoc.ActiveDoc.Views.ActiveView;

			view.ActiveViewport.SetCameraTarget(targetPoint, true);
			view.ActiveViewport.SetCameraLocations(targetPoint, cameraPoint);

			view.ActiveViewport.Rotate(horizontalAngle, Vector3d.ZAxis, cameraPoint);
			view.ActiveViewport.Rotate(verticalAngle, Vector3d.XAxis, cameraPoint);
		}

		public override Guid ComponentGuid => new Guid("119E066B-FBE2-41CD-BFAD-7868F9A10C80");
	}
}