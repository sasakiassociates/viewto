using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;
using ViewTo.RhinoGh.Properties;
using PointCloud = Rhino.Geometry.PointCloud;

namespace ViewTo.RhinoGh.Points
{

  public class PointGenGround : GH_Component
  {
    private readonly double scaleFactor = RhinoMath.UnitScale(UnitSystem.Meters, RhinoDoc.ActiveDoc.ModelUnitSystem);

    private int _iMeshes, _iRegion, _iHeight, _iSpacing, _iCountZ; // _iStepX, _iStepY, _iStepZ;
    private PointCloud _pc;

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      var index = 0;
      pManager.AddMeshParameter("Ground", "G", "Ground geometry over which points are generated.", GH_ParamAccess.list);
      _iMeshes = index++;
      pManager.AddCurveParameter("Region", "R", "Optional region within which points are generated.", GH_ParamAccess.item);
      _iRegion = index++;
      pManager.AddNumberParameter("Start Height", "H", "Starting height above ground to generate points. Default is 1.8m.", GH_ParamAccess.item, 1.8 * scaleFactor);
      _iHeight = index++;
      pManager.AddNumberParameter("Spacing", "S", "Spacing between points. Default is 3m.", GH_ParamAccess.item, 3 * scaleFactor);
      _iSpacing = index++;
      //pManager.AddNumberParameter( "Step Size X", "X", "Point spacing in the X direction. Default is 10m.", GH_ParamAccess.item, 10.0 );
      //_iStepX = index++;
      //pManager.AddNumberParameter( "Step Size Y", "Y", "Point spacing in the Y direction. Default is 10m.", GH_ParamAccess.item, 10.0 );
      //_iStepY = index++;
      pManager.AddIntegerParameter("Z Count", "Z", "Number of layers of points. Default is 1.", GH_ParamAccess.item, 1);
      _iCountZ = index;

      pManager[_iRegion].Optional = true;
      pManager[_iHeight].Optional = true;
      pManager[_iSpacing].Optional = true;
      //pManager[ _iStepX ].Optional = true;
      //pManager[ _iStepY ].Optional = true;
      pManager[_iCountZ].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddPointParameter("Viewpoints", "P", "Viewpoints to be analyzed", GH_ParamAccess.list);
    }

    public override void DrawViewportWires(IGH_PreviewArgs args)
    {
      if (_pc != null)
        args.Display.DrawPointCloud(_pc, 2);
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // Get grounds
      var grounds = new List<Mesh>();
      if (!DA.GetDataList(_iMeshes, grounds)) return;

      //grounds = grounds.Where(x => x != null).ToList();
      for (var i = grounds.Count - 1; i >= 0; i--)
        if (grounds[i] == null)
        {
          grounds.RemoveAt(i);
          this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Ignoring null ground geometry.");
          if (grounds.Count == 0)
          {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No valid ground geometry supplied.");
            return;
          }
        }

      var bounds = new BoundingBox();
      var mergedMesh = new Mesh();

      foreach (var m in grounds)
      {
        bounds.Union(m.GetBoundingBox(false));
        mergedMesh.Append(m);
      }

      Curve region = null;
      if (!DA.GetData(_iRegion, ref region))
      {
        Line[] edges = mergedMesh.GetBoundingBox(false).GetEdges();
        region = Curve.JoinCurves(edges.Select(x => x.ToNurbsCurve())).FirstOrDefault();
      }

      var heightFromGround = new double();
      DA.GetData(_iHeight, ref heightFromGround);

      var distanceBetweenPoints = new double();
      DA.GetData(_iSpacing, ref distanceBetweenPoints);

      //double stepSizeX = new double( );
      //double stepSizeY = new double( );

      //DA.GetData( _iStepX, ref stepSizeX );
      //DA.GetData( _iStepY, ref stepSizeY );

      #region Error Handling From Prev
      if (distanceBetweenPoints < 0.3 * scaleFactor)
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Step Size X is too small - adjusting to 0.3m.");
        distanceBetweenPoints = 0.3;
      }

      double area = grounds.Select(x => AreaMassProperties.Compute(x, true, false, false, false).Area).Sum();

      if (area / (distanceBetweenPoints * distanceBetweenPoints) > 1000000)
        if (MessageBox.Show(
              string.Concat("Lots and lots of points to generate! Could take a while...Continue?"),
              string.Concat(this.Name, " Warning"),
              MessageBoxButtons.YesNo)
            != DialogResult.Yes)
        {
          this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Operation canceled. Try a smaller ground region or larger point spacing.");
          return;
        }
      #endregion

      var zCount = 0;
      DA.GetData(_iCountZ, ref zCount);

      var points = new List<Point3d>();

      if (region != null)
      {
        // TODO fix so this does not get triggered each update 
        points = mergedMesh.PointGridProjection(region, distanceBetweenPoints, distanceBetweenPoints);

        if (zCount > 0 && distanceBetweenPoints > double.MinValue)
        {
          var offsetPoints = new List<Point3d>();

          foreach (var ptn in points)
          {
            var location = ptn;
            for (var z = 0; z < zCount; z++)
            {
              location.Z = ptn.Z + heightFromGround + z * distanceBetweenPoints;
              offsetPoints.Add(new Point3d(location));
            }
          }

          points = offsetPoints;
        }
        _pc = new PointCloud(points);
        DA.SetDataList(0, points);


      }
    }

    #region Component Info
    public PointGenGround() : base(
      "Ground Point Generator", "GPG",
      "Generate Viewpoints on Ground",
      ConnectorInfo.CATEGORY, ConnectorInfo.Nodes.CLOUD)
    { }

    protected override Bitmap Icon => new Bitmap(Icons.GeneratePointGround);

    public override Guid ComponentGuid => new Guid("c1c4f971-2b19-43b5-b753-606e85aa323e");
    #endregion

  }
}
