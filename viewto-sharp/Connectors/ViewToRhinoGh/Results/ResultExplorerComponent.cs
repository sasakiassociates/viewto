using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ViewObjects;
using ViewObjects.Clouds;
using Sasaki.Common;
using ViewObjects.Contents;
using ViewObjects.Results;
using ViewObjects.Studies;
using ViewTo.RhinoGh.Points;
using ViewTo.RhinoGh.Properties;

namespace ViewTo.RhinoGh.Results
{

  public class ResultExplorerComponent : ViewToCloudComponentBase
  {
    // TODO: Allow for remapping colors to a range of values 
    // TODO: Setting value type for exploring 

    const int MAX_ALPHA = 255;

    const int MIN_ALPHA = 100;

    IExplorer _explorer;
    IViewStudy _study;

    (int Obj, int OptionA, int OptionB, int ValueType, int Settings, int Mask, int NormalizeByMask, int Size, int PixelRange, int ValueRange) _input;
    // private double _min = 1.0, _max;

    private(int Points, int Colors, int Values, int ActivePoint, int ActiveValue, int ActiveColor) _output;
    private ExplorerSettings _settings;

    private ExplorerValueType _valueType = ExplorerValueType.ExistingOverPotential;

    public ResultExplorerComponent() : base("Result Explorer", "EX", "Explore a set of view study results", ConnectorInfo.Nodes.EXPLORER)
    {
      _explorer = new Explorer();
    }

    public override Guid ComponentGuid => new Guid("01ffe845-0a7b-4bf8-9d35-48f234fc8cfc");

    protected override Bitmap Icon => new Bitmap(Icons.ExploreResults);

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      var index = 0;
      pManager.AddGenericParameter("ViewStudy", "V", "View Study to Explore", GH_ParamAccess.item);
      _input.Obj = index++;

      pManager.AddGenericParameter("Option A", "A", "Content Option to Explore", GH_ParamAccess.item);
      _input.OptionA = index++;

      pManager.AddGenericParameter("Option B", "B", "Content Option to Explore", GH_ParamAccess.item);
      _input.OptionB = index++;

      pManager.AddGenericParameter("Explorer Settings", "S", "Explorer Input Settings", GH_ParamAccess.item);
      _input.Settings = index++;

      pManager.AddMeshParameter("Mask", "M", "Masking area to filter points by form", GH_ParamAccess.list);
      _input.Mask = index++;

      pManager.AddBooleanParameter("Normalize Mask", "NM", "Nomarlize the values by the masked point set", GH_ParamAccess.item, true);
      _input.NormalizeByMask = index++;

      pManager.AddIntegerParameter("Point Size", "S", "Size of Point in Cloud", GH_ParamAccess.item, 3);
      _input.Size = index++;

      pManager.AddIntervalParameter("Value", "V", "Min and Max normalized value to filter by", GH_ParamAccess.item, new Interval(0.0, 1.0));
      _input.ValueRange = index++;

      pManager.AddIntervalParameter("Pixel Range", "P", "Min and Max value clamp the values by", GH_ParamAccess.item, new Interval(0, ViewCoreExtensions.MAX_PIXEL_COUNT));
      _input.PixelRange = index;

      pManager[_input.OptionB].Optional = true;
      pManager[_input.NormalizeByMask].Optional = true;
      pManager[_input.Mask].Optional = true;
      pManager[_input.Size].Optional = true;
      pManager[_input.PixelRange].Optional = true;
      pManager[_input.ValueRange].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      var index = 0;

      pManager.AddPointParameter("Points", "P", "Points from filter", GH_ParamAccess.tree);
      _output.Points = index++;

      pManager.AddColourParameter("Color", "C", "Color from filter", GH_ParamAccess.tree);
      _output.Colors = index++;

      pManager.AddNumberParameter("Values", "V", "Values from filter", GH_ParamAccess.tree);
      _output.Values = index++;

      pManager.AddPointParameter("Active Point", "AP", "Active point the explorer is using", GH_ParamAccess.item);
      _output.ActivePoint = index++;

      pManager.AddNumberParameter("Active Value", "AV", "Active value for the result point", GH_ParamAccess.item);
      _output.ActiveValue = index++;

      pManager.AddColourParameter("Active Color", "AC", "Active color for the result point", GH_ParamAccess.item);
      _output.ActiveColor = index;
    }


    protected override void SolveInstance(IGH_DataAccess DA)
    {
      DA.GetData(_input.Size, ref pointSize);

      var normalizeMask = true;
      DA.GetData(_input.NormalizeByMask, ref normalizeMask);

      var valueRange = new Interval();
      DA.GetData(_input.ValueRange, ref valueRange);

      var pixelRange = new Interval();
      DA.GetData(_input.PixelRange, ref pixelRange);


      var maskTree = new List<GH_Mesh>();
      DA.GetDataList(_input.Mask, maskTree);

      GH_ObjectWrapper ghWrapper = default;
      DA.GetData(_input.Settings, ref ghWrapper);

      if(ghWrapper?.Value is ExplorerSettings settings)
      {
        _settings = settings;
        _valueType = _settings.valueType;
      }
      else
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No Settings passed");
        return;
      }



      GH_ObjectWrapper wrapper = null;
      ContentOption opA = default;
      ContentOption opB = default;
      DA.GetData(_input.OptionA, ref wrapper);
      if(wrapper?.Value is ContentOption oa)
      {
        opA = oa;
      }
      else
      {
        return;
      }
      DA.GetData(_input.OptionB, ref wrapper);
      if(wrapper?.Value is ContentOption ob)
      {
        opB = ob;
      }
      else
      {
        return;
      }

      // load cloud point
      DA.GetData(_input.Obj, ref wrapper);

      double[] explorerValues = null;
      if(wrapper?.Value is ViewStudy obj)
      {

        if(_study == default(object) || !_study.guid.Equals(obj.guid))
        {
          _study = obj;
        }

        var rc = _study.FindObject<ResultCloud>();
        if(rc == null)
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, $"No {nameof(ResultCloud)} found in this study");
          return;
        }

        _explorer.Load(rc);
      }

      var filteredPoints = new Dictionary<int, Point3d>();

      // cast all points 
      for(var i = 0; i<_explorer.cloud.Points.Length; i++) filteredPoints.Add(i, _explorer.cloud.Points[i].ToRhino());

      if(!settings.showAll && maskTree.Any())
      {

        for(int index = filteredPoints.Keys.Count-1; index>=0; index--)
        {
          var isIn = false;

          foreach(var mask in maskTree)
          {

            if(mask.Value.IsPointInside(filteredPoints[index], double.MinValue, false))
            {
              isIn = true;
              break;
            }
          }

          // remove points that not within the scope
          if(!isIn) filteredPoints.Remove(index);

        }
      }

      var filter = new ExplorerFilterInput(valueRange.Min, valueRange.Max, (int)pixelRange.Min, (int)pixelRange.Max, filteredPoints.Keys.ToArray());

      explorerValues = _explorer.GetSols(opA, opB, filter, normalizeMask).ToArray();

      if(!explorerValues.Any())
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Explorer did not load result cloud properly!");
        return;
      }

      var colors = new List<Color>();
      var values = new List<double>();
      var points = new List<Point3d>();

      var flattenedPoints = filteredPoints.Values.ToList();
      for(int i = flattenedPoints.Count-1; i>=0; i--)
      {
        var value = explorerValues[i];

        if(_settings.showAll || _settings.InRange(value))
        {
          colors.Add(Color.FromArgb(MAX_ALPHA, _settings.GetColor(value)));
          values.Add(value);
          points.Add(flattenedPoints[i]);
        }

      }

      renderedCloud = new PointCloud();
      renderedCloud.ClearColors();

      for(var i = 0; i<points.Count; i++)
        renderedCloud.Add(points[i], colors[i]);

      DA.SetDataList(_output.Points, points);
      DA.SetDataList(_output.Colors, colors);
      DA.SetDataList(_output.Values, values);

      var activePoint = Math.Min(_settings.point, points.Count-1);

      DA.SetData(_output.ActivePoint, _explorer.cloud.Points[activePoint].ToRhino());
      DA.SetData(_output.ActiveValue, explorerValues[activePoint]);
      DA.SetData(_output.ActiveColor, _settings.GetColor(explorerValues[_settings.point]));

    }

  }

}
