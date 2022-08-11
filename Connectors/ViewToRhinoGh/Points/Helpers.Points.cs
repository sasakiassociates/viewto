using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel.Types;
using Rhino;
using Rhino.Geometry;
using ViewObjects;
using Plane = Rhino.Geometry.Plane;

namespace ViewTo.RhinoGh.Points
{
	public static class Helpers
	{

		public static GH_Point ToGrass(this CloudPoint p) => new GH_Point(p.ToRhino());

		public static Point3d ToRhino(this CloudPoint p) => new Point3d(p.x, p.y, p.z);

		// NOTE pulled from OG point creation script
		// TODO move point creation logic into async call to avoid blocking ui 
		public static IEnumerable<Point3d> CreateXYGrid(Curve region, double stepSizeX, double stepSizeY)
		{
			var pointsToProject = new List<Point3d>(); // XY grid of points projected onto ground mesh

			if (region == null) return pointsToProject;

			// base bounding box off of curve input
			region.GetBoundingBox(Plane.WorldXY, out var regionBox);

			// var dir = CreateBoundDir( regionBox );

			// get anchor point of region
			double regionWidthStart = regionBox.X.Min;
			double regionDepthStart = regionBox.Y.Min;

			// redundant or unused
			// double regionWidthEnd = regionBox.X.Max;
			// double regionDepthEnd = regionBox.Y.Max;
			// double regionDepth = Math.Abs( regionBox.Y.Length );

			// create number of steps by box size
			var widthSteps = (int)(Math.Abs(regionBox.X.Length) / stepSizeX);
			var depthStepCount = (int)(Math.Abs(regionBox.Y.Length) / stepSizeY);

			for (var i = 0; i < widthSteps; i++)
			for (var j = 0; j < depthStepCount; j++)
			{
				// set up base grid XY coordinates
				var x = regionWidthStart + i * stepSizeX;
				var y = regionDepthStart + j * stepSizeY;
				Point3d xyPt = new Point3d(x, y, 0);

				// test for region inclusion

				if (region.Contains(xyPt, Plane.WorldXY, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance) == PointContainment.Inside) pointsToProject.Add(xyPt);
			}

			return pointsToProject;
		}

		public static List<Point3d> CreateCloudWithSpacing(BoundingBox bb, double xSpacing, double ySpacing, double zSpacing)
		{
			var minX = bb.Min.X;
			var maxX = bb.Max.X;
			var minY = bb.Min.Y;
			var maxY = bb.Max.Y;
			var minZ = bb.Min.Z;
			var maxZ = bb.Max.Z;

			var xSpan = maxX - minX;
			var ySpan = maxY - minY;
			var zSpan = maxZ - minZ;

			var xCount = 1.0;
			var yCount = 1.0;
			var zCount = 1.0;

			if (xSpacing < xSpan)
				xCount = xSpan / xSpacing;
			if (ySpacing < ySpan)
				yCount = ySpan / ySpacing;
			if (zSpacing < zSpan)
				zCount = zSpan / zSpacing;

			var cloud = new List<Point3d>();

			for (var x = 0; x < xCount; x++)
			{
				var xPos = minX + x * xSpacing;

				for (var y = 0; y < yCount; y++)
				{
					var yPos = minY + y * ySpacing;

					for (var z = 0; z < zCount; z++)
					{
						var zPos = minZ + z * zSpacing;

						cloud.Add(new Point3d(xPos, yPos, zPos));
					}
				}
			}

			return cloud;
		}

		private static Vector3d CreateBoundDir(BoundingBox bounds)
		{
			Line lineDirection = new Line(bounds.Corner(true, true, false), bounds.Corner(true, true, true));
			Vector3d dir = lineDirection.Direction;
			dir.Unitize();
			return dir;
		}

		public static List<Point3d> PointGridProjection(this Mesh ground, Curve region, double stepSizeX, double stepSizeY)
		{
			var projectedPoints = new List<Point3d>();

			try
			{
				region.GetBoundingBox(Plane.WorldXY, out var regionBox);

				var dir = regionBox.BoundingBox.Diagonal;
				var lineSize = dir.Length;
				dir.Unitize();

				foreach (var ptn in CreateXYGrid(region, stepSizeX, stepSizeY))
				{
					//var line = new Line(ptn, dir, lineSize);)
					//var hits = Rhino.Geometry.Intersect.Intersection.MeshLine(ground, line, out var faces);
					// NOTE this grabs the first point in x-z...but seems to be buggy  
					// Point3d projPt = projectedPtn.OrderByDescending( x => x.Z ).First( );
					Point3d[] hits = Rhino.Geometry.Intersect.Intersection.ProjectPointsToMeshes(new List<Mesh>
					{
						ground
					}, new List<Point3d>
					{
						ptn
					}, Vector3d.ZAxis, 0.0001);
					if (hits != null && hits.Length > 0)
					{
						Point3d projPt = hits.LastOrDefault();
						projectedPoints.Add(projPt);
					}
				}
			}
			catch (Exception e)
			{
				// TODO actually throw something 
				Console.WriteLine(e);
				throw;
			}

			return projectedPoints;
		}
	}
}