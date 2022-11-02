using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ViewTo.RhinoGh.Properties;
namespace ViewTo.RhinoGh.Points
{
  public class PointGenCloud : ViewToCloudComponentBase
  {

    private (int SpacingX, int SpacingY, int SpacingZ, int Bounds, int Mask) _input;

    private (int Masked, int UnMasked) _output;

    public PointGenCloud() : base(
      "Cloud Point Generator", "CPG",
      "Generate A Cloud of Points",
      ConnectorInfo.Nodes.CLOUD)
    {
    }

    protected override Bitmap Icon => new Bitmap(Icons.GeneratePointsCloud);

    public override Guid ComponentGuid => new Guid("12e9f6ab-4e35-4076-b17f-74fc42e2c3f2");

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      var index = 0;
      pManager.AddGeometryParameter("Geometry Bounds", "B", "Geometry to build bounds around", GH_ParamAccess.list);
      _input.Bounds = index++;
      pManager.AddMeshParameter("Masking Meshes", "M", "Meshes to mask points with", GH_ParamAccess.list);
      _input.Mask = index++;
      pManager.AddNumberParameter("Step Size X", "X", "Grid count for X", GH_ParamAccess.item, 1.0);
      _input.SpacingX = index++;
      pManager.AddNumberParameter("Step Size Y", "Y", "Grid count for Y", GH_ParamAccess.item, 1.0);
      _input.SpacingY = index++;
      pManager.AddNumberParameter("Step Size Z", "Z", "Grid count for Z", GH_ParamAccess.item, 1.0);
      _input.SpacingZ = index;

      pManager[_input.Mask].Optional = true;
      pManager[_input.SpacingX].Optional = true;
      pManager[_input.SpacingY].Optional = true;
      pManager[_input.SpacingZ].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      var index = 0;
      pManager.AddPointParameter("Points in Mask", "P", "Points that were found in the mask!", GH_ParamAccess.list);
      _output.Masked = index++;
      pManager.AddPointParameter("UnMask Points", "U", "Points that were found outside the mask!", GH_ParamAccess.list);
      _output.UnMasked = index;
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      var geo = new List<GeometryBase>();
      if (!DA.GetDataList(_input.Bounds, geo))
      {
        return;
      }

      var bb = new BoundingBox();
      foreach (var g in geo.Where(g => g != null))
      {
        bb.Union(g.GetBoundingBox(false));
      }

      double xSpacing = 0;
      DA.GetData(_input.SpacingX, ref xSpacing);
      double ySpacing = 0;
      DA.GetData(_input.SpacingY, ref ySpacing);
      double zSpacing = 0;
      DA.GetData(_input.SpacingZ, ref zSpacing);

      // NOTE heavy on performance
      var cloud = Helpers.CreateCloudWithSpacing(bb, xSpacing, ySpacing, zSpacing);

      var meshes = new List<Mesh>();
      DA.GetDataList(_input.Mask, meshes);

      renderedCloud = new PointCloud();

      if (meshes.Count > 0)
      {
        var maskedPoints = new List<Point3d>();
        var unmasked = new List<Point3d>();

        var mergedMesh = new Mesh();
        foreach (var m in meshes)
        {
          mergedMesh.Append(m);
        }

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

          renderedCloud.Add(p, color);
        }

        DA.SetDataList(_output.Masked, maskedPoints);
        DA.SetDataList(_output.UnMasked, maskedPoints);
      }
      else
      {
        // NOTE if no mask
        renderedCloud.AddRange(cloud);
        DA.SetDataList(_output.UnMasked, cloud);
      }
    }
  }
}
