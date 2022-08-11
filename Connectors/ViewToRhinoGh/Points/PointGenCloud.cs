using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;
using ViewTo.RhinoGh.Properties;
using PointCloud = Rhino.Geometry.PointCloud;

namespace ViewTo.RhinoGh.Points
{
	public class PointGenCloud : GH_Component
	{

		private int _iSpacingX, _iSpacingY, _iSpacingZ, _iBounds, _iMask;

		private int _oMasked, _oUnMasked;
		private PointCloud _pc;

		protected override void RegisterInputParams(GH_InputParamManager pManager)
		{
			var index = 0;
			pManager.AddGeometryParameter("Geometry Bounds", "B", "Geometry to build bounds around", GH_ParamAccess.list);
			_iBounds = index++;
			pManager.AddMeshParameter("Masking Meshes", "M", "Meshes to mask points with", GH_ParamAccess.list);
			_iMask = index++;
			pManager.AddNumberParameter("Step Size X", "X", "Grid count for X", GH_ParamAccess.item, 1.0);
			_iSpacingX = index++;
			pManager.AddNumberParameter("Step Size Y", "Y", "Grid count for Y", GH_ParamAccess.item, 1.0);
			_iSpacingY = index++;
			pManager.AddNumberParameter("Step Size Z", "Z", "Grid count for Z", GH_ParamAccess.item, 1.0);
			_iSpacingZ = index;

			pManager[_iMask].Optional = true;
			pManager[_iSpacingX].Optional = true;
			pManager[_iSpacingY].Optional = true;
			pManager[_iSpacingZ].Optional = true;
		}

		protected override void RegisterOutputParams(GH_OutputParamManager pManager)
		{
			var index = 0;
			pManager.AddPointParameter("Points in Mask", "P", "Points that were found in the mask!", GH_ParamAccess.list);
			_oMasked = index++;
			pManager.AddPointParameter("UnMask Points", "U", "Points that were found outside the mask!", GH_ParamAccess.list);
			_oUnMasked = index;
		}

		public override void DrawViewportWires(IGH_PreviewArgs args)
		{
			if (_pc != null)
				args.Display.DrawPointCloud(_pc, 2);
		}

		protected override void SolveInstance(IGH_DataAccess DA)
		{
			var geo = new List<GeometryBase>();
			if (!DA.GetDataList(_iBounds, geo)) return;

			var bb = new BoundingBox();
			foreach (var g in geo.Where(g => g != null)) bb.Union(g.GetBoundingBox(false));

			double xSpacing = 0;
			DA.GetData(_iSpacingX, ref xSpacing);
			double ySpacing = 0;
			DA.GetData(_iSpacingY, ref ySpacing);
			double zSpacing = 0;
			DA.GetData(_iSpacingZ, ref zSpacing);

			// NOTE heavy on performance
			var cloud = Helpers.CreateCloudWithSpacing(bb, xSpacing, ySpacing, zSpacing);

			var meshes = new List<Mesh>();
			DA.GetDataList(_iMask, meshes);

			if (_pc == null)
				_pc = new PointCloud();

			if (meshes != null && meshes.Count > 0)
			{
				var maskedPoints = new List<Point3d>();
				var unmasked = new List<Point3d>();

				var mergedMesh = new Mesh();
				foreach (var m in meshes) mergedMesh.Append(m);

				foreach (var p in cloud)
				{
					var color = Color.White;
					if (mergedMesh.IsPointInside(p, 0.001, false))
					{
						maskedPoints.Add(p);
						color = Color.Aqua;
					}
					else
					{
						unmasked.Add(p);
					}

					_pc.Add(p, color);
				}

				DA.SetDataList(_oMasked, maskedPoints);
				DA.SetDataList(_oUnMasked, maskedPoints);
			}
			else
			{
				// NOTE if no mask
				_pc.AddRange(cloud);
				DA.SetDataList(_oUnMasked, cloud);
			}
		}

		#region Component Info

		public PointGenCloud() : base(
			"Cloud Point Generator", "CPG",
			"Generate A Cloud of Points",
			ConnectorInfo.CATEGORY, ConnectorInfo.Nodes.CLOUD)
		{ }

		protected override Bitmap Icon => new Bitmap(Icons.GeneratePointsCloud);

		public override Guid ComponentGuid => new Guid("12e9f6ab-4e35-4076-b17f-74fc42e2c3f2");

		#endregion

	}
}