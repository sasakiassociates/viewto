using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino;
using Rhino.Geometry;
using ViewTo.RhinoGh.Properties;

namespace ViewTo.RhinoGh.Points
{
	public class PointGenFacade : ViewToCloudComponentBase
	{

		public PointGenFacade() :
			base(
				"Facade Point Generator",
				"FPG",
				"Generate Viewpoints on Building",
				ConnectorInfo.Nodes.CLOUD)
		{ }

		protected override Bitmap Icon => new Bitmap(Icons.GeneratePointFacade);

		public override Guid ComponentGuid => new Guid("88e7a8a0-3fa1-4ed3-8e66-a0a9237a567a");

		protected override void RegisterInputParams(GH_InputParamManager pManager)
		{
			pManager.AddGeometryParameter("Buildings", "B", "Buildings to contour for points.", GH_ParamAccess.list);

			pManager.AddNumberParameter("Step Size W", "W", "Spacing in between points. Default is 4m.", GH_ParamAccess.item, 4.0);
			pManager.AddNumberParameter("Step Size H", "H", "Height in between points. Default is 4m.", GH_ParamAccess.item, 4.0);

			pManager[1].Optional = true;
			pManager[2].Optional = true;
		}

		protected override void RegisterOutputParams(GH_OutputParamManager pManager)
		{
			pManager.AddPointParameter("Viewpoints", "P", "Viewpoints to be analyzed", GH_ParamAccess.list);
			pManager.AddVectorParameter("View normals", "N", "Vector normals to be analyzed", GH_ParamAccess.list);
		}

		protected override void SolveInstance(IGH_DataAccess DA)
		{
			#region get input data

			List<GeometryBase> buildings = new List<GeometryBase>();

			var stepSizeX = new double();
			var stepSizeZ = new double();

			if (!DA.GetDataList(0, buildings)) return;
			if (!DA.GetData(1, ref stepSizeX)) return;
			if (!DA.GetData(2, ref stepSizeZ)) return;

			if (stepSizeX <= 0 || stepSizeZ <= 0)
			{
				AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Dimensions must be positive, non-zero numbers.");
				return;
			}

			if (stepSizeX < 0.3)
			{
				AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Step Size X is too small - adjusting to 0.3.");
				stepSizeX = 0.3;
			}

			if (stepSizeZ < 0.3)
			{
				AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Step Size Z is too small - adjusting to 0.3.");
				stepSizeZ = 0.3;
			}

			// make sure that default (meter) dimensions are scaled correctly
			double scaleFactor =
				RhinoMath.UnitScale(UnitSystem.Meters, RhinoDoc.ActiveDoc.ModelUnitSystem); // value for converting meters into active doc units
			stepSizeX *= scaleFactor;
			stepSizeZ *= scaleFactor;

			#endregion

			#region initialize

			// output data
			List<GH_Point> viewPoints = new List<GH_Point>();
			List<GH_Vector> viewNormals = new List<GH_Vector>();

			// internal data
			renderedCloud = new PointCloud();
			var intersectionTolerance = 0.01;

			#endregion

			//---------------------------------BUILDING CONTOURS---------------------------------//
			foreach (GeometryBase building in buildings)
			{
				if (building == null) continue;

				// get starting point (at top of building)
				BoundingBox box = building.GetBoundingBox(false);
				var z = box.Max.Z - stepSizeZ / 2;
				// "top of building" is defined as half a z step size (half a floor) below the top of the geometry
				Point3d buildingPt = new Point3d(box.Center.X, box.Center.Y, z);

				// iterate down level by level from top, until bottom of geometry is reached
				var count = 0;
				while (z > box.Min.Z)
				{
					// create intersection plane at that level
					Point3d levelPoint = new Point3d(buildingPt.X, buildingPt.Y, z);
					Plane levelPlane = new Plane(levelPoint, Vector3d.ZAxis);

					// intersect plane with geometry to get level curves 
					Curve[] intersectionCurves;
					Point3d[] intersectionPoints;

					switch (building.ObjectType)
					{
						case Rhino.DocObjects.ObjectType.Brep:
						case Rhino.DocObjects.ObjectType.Extrusion:
							Rhino.Geometry.Intersect.Intersection.BrepPlane((Brep)building,
							                                                levelPlane,
							                                                intersectionTolerance,
							                                                out intersectionCurves,
							                                                out intersectionPoints);
							break;
						case Rhino.DocObjects.ObjectType.Mesh:
							intersectionCurves = Rhino.Geometry.Intersect.Intersection.MeshPlane((Mesh)building, levelPlane).Select(x => x.ToNurbsCurve())
								.ToArray();
							break;
						default:
							continue;
					}

					// get points from each level curve
					foreach (Curve curve in intersectionCurves)
					{
						// get number of steps based on length of curve and step size
						double length = curve.GetLength(RhinoDoc.ActiveDoc.ModelAbsoluteTolerance);
						var steps = (int)(length / stepSizeX);

						// iterate along curve to get points
						for (var x = 0; x <= steps; x++)
						{
							Point3d curvePoint = curve.PointAtLength(x * stepSizeX);

							// get normal vector of surface at that point - used to make sure point is sufficiently outside of building
							Point3d surfacePoint;
							ComponentIndex ci;
							double t, s;
							Vector3d surfaceNormal = new Vector3d();

							if (building.ObjectType == Rhino.DocObjects.ObjectType.Brep || building.ObjectType == Rhino.DocObjects.ObjectType.Extrusion)
							{
								Brep _building = (Brep)building;
								bool normal = _building.ClosestPoint(curvePoint, out surfacePoint, out ci, out s, out t, 1, out surfaceNormal);
							}
							else if (building.ObjectType == Rhino.DocObjects.ObjectType.Mesh)
							{
								Mesh _building = (Mesh)building;
								MeshPoint meshPoint = _building.ClosestMeshPoint(curvePoint, 1);
								surfaceNormal = _building.NormalAt(meshPoint);
							}

							// move point just a little bit outside of geometry (0.1m) and add to list
							surfaceNormal = new Vector3d(surfaceNormal.X, surfaceNormal.Y, 0);
							surfaceNormal.Unitize();
							viewNormals.Add(new GH_Vector(surfaceNormal));
							surfaceNormal *= 0.1 * scaleFactor;
							curvePoint += surfaceNormal;

							renderedCloud.Add(curvePoint);
							viewPoints.Add(new GH_Point(curvePoint));
						}
					}

					// make sure while loop doesn't go underneath mesh...
					if (count > 1000)
					{
						AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "something's wrong...too many points logged");
						break;
					}

					count++;
					z -= stepSizeZ;
				}
			}

			#region output

			DA.SetDataList(0, viewPoints);
			DA.SetDataList(1, viewNormals);

			#endregion
		}

	}
}