using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;
using ViewTo.RhinoGh.Properties;

namespace ViewTo.RhinoGh.Points
{
	public class PointGenFittedCloud : GH_Component
	{

		int _iBounds, _iCountX, _iCountY, _iCountZ;

		PointCloud _pc;

		protected override void RegisterInputParams(GH_InputParamManager pManager)
		{
			var index = 0;
			pManager.AddGeometryParameter("Geometry Bounds", "G", "Bounds of Geometry to create cloud within", GH_ParamAccess.list);
			_iBounds = index++;
			pManager.AddIntegerParameter("X Count", "X", "Amount of steps to take in X direction", GH_ParamAccess.item, 5);
			_iCountX = index++;
			pManager.AddIntegerParameter("Y Count", "Y", "Amount of steps to take in Y direction", GH_ParamAccess.item, 5);
			_iCountY = index++;
			pManager.AddIntegerParameter("Z Count", "Z", "Amount of steps to take in Z direction", GH_ParamAccess.item, 5);
			_iCountZ = index;

			pManager[_iCountX].Optional = true;
			pManager[_iCountY].Optional = true;
			pManager[_iCountZ].Optional = true;
		}

		protected override void RegisterOutputParams(GH_OutputParamManager pManager)
		{
			pManager.AddPointParameter("Points", "P", "Points created", GH_ParamAccess.list);
		}

		public override void DrawViewportWires(IGH_PreviewArgs args)
		{
			if (_pc != null) args.Display.DrawPointCloud(_pc, 1);
		}

		protected override void SolveInstance(IGH_DataAccess DA)
		{
			var geo = new List<GeometryBase>();
			if (!DA.GetDataList(_iBounds, geo)) return;

			var bb = new BoundingBox();
			foreach (var g in geo.Where(g => g != null)) bb.Union(g.GetBoundingBox(false));

			var xCount = 0;
			DA.GetData(_iCountX, ref xCount);
			var yCount = 0;
			DA.GetData(_iCountY, ref yCount);
			var zCount = 0;
			DA.GetData(_iCountZ, ref zCount);

			var cloud = new List<Point3d>();

			var xCrv = new LineCurve(new Line(bb.Corner(true, true, true), bb.Corner(false, true, true)));
			var yCrv = new LineCurve(new Line(bb.Corner(true, true, true), bb.Corner(true, false, true)));
			var zCrv = new LineCurve(new Line(bb.Corner(true, true, true), bb.Corner(true, true, false)));

			var xSize = xCrv.DivideByCount(xCount, true);
			var ySize = yCrv.DivideByCount(yCount, true);
			var zSize = zCrv.DivideByCount(zCount, true);

			var minX = bb.Min.X;
			var maxX = bb.Max.X;
			var minY = bb.Min.Y;
			var maxY = bb.Max.Y;
			var minZ = bb.Min.Z;
			var maxZ = bb.Max.Z;

			for (var x = 0; x < xCount; x++)
			{
				var xPos = CheckPos(x, xCount, minX, maxX, xSize[x]);

				for (var y = 0; y < yCount; y++)
				{
					var yPos = CheckPos(y, yCount, minY, maxY, ySize[y]);

					for (var z = 0; z < zCount; z++)
					{
						var zPos = CheckPos(z, zCount, minZ, maxZ, zSize[z]);

						cloud.Add(new Point3d(xPos, yPos, zPos));
					}
				}
			}

			if (_pc == null)
				_pc = new PointCloud();

			_pc.AddRange(cloud);

			DA.SetDataList(0, cloud);
		}

		static double CheckPos(int index, int count, double min, double max, double size) => index >= count ? max : min + index + size;

		#region Component Info

		public PointGenFittedCloud() : base(
			"Fitted Point Cloud Generator", "FPG",
			"Generate a point cloud that is nice and tight!",
			ConnectorInfo.CATEGORY, ConnectorInfo.Nodes.CLOUD)
		{ }

		protected override Bitmap Icon
		{
			get => new Bitmap(Icons.GeneratePointGround);
		}

		public override Guid ComponentGuid
		{
			get => new Guid("17BAE830-6740-4854-BF67-E1F78A4C455E");
		}

		#endregion

	}
}