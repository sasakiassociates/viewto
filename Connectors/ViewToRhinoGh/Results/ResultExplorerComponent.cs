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
using ViewObjects.Common;
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

    (int Obj, int OptionA, int OptionB, int ValueType, int Settings, int Mask, int NormalizeByMask, int Size) _input;
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

      pManager.AddIntegerParameter("Point Size", "P", "Size of Point in Cloud", GH_ParamAccess.item, 3);
      _input.Size = index;

      pManager[_input.OptionB].Optional = true;
      pManager[_input.NormalizeByMask].Optional = true;
      pManager[_input.Mask].Optional = true;
      pManager[_input.Size].Optional = true;
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

    // private void SetMinMax(double[] value)
    // {
    //   _min = 1.0;
    //   _max = 0.0;
    //
    //   if(value.Valid())
    //     foreach(var t in value)
    //       if(!double.IsNaN(t))
    //         SetMinMax(t);
    // }
    //
    // private void SetMinMax(double value)
    // {
    //   // values that have no view are set to -1
    //   if(value<0 || double.IsNaN(value)) return;
    //
    //   if(value<_min) _min = value;
    //   if(value>_max) _max = value;
    // }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      DA.GetData(_input.Size, ref pointSize);

      var normalizeMask = true;
      DA.GetData(_input.NormalizeByMask, ref normalizeMask);

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

        if(_study == default(object) || !_study.ViewId.Equals(obj.ViewId))
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
            // remove points that not within the scope
            if(!isIn) filteredPoints.Remove(index);
          }
        }
      }

      if(!normalizeMask)
      {
        if(_explorer.TryGetSols(opA, opB, out var normValues))
        {
          explorerValues = normValues.ToArray();
        }
      }
      else
      {
        if(_explorer.TryGetSols(opA, opB, filteredPoints.Keys.ToList(), out var maskedValues))
        {
          explorerValues = maskedValues.ToArray();
        }
      }


      if(explorerValues == null || !explorerValues.Any())
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Explorer did not load result cloud properly!");
        return;
      }

    #region shit to remove

      // if(maskTree != null && !maskTree.IsEmpty)
      // {
      //   for(var i = 0; i<explorerValues.Length; i++)
      //   {
      //     var point = _explorer.cloud.Points[i].ToGrass();
      //
      //     for(var treeIndex = 0; treeIndex<maskTree.Branches.Count; treeIndex++)
      //     {
      //       var branch = maskTree.Branches[treeIndex];
      //       var path = maskTree.Paths[treeIndex];
      //
      //       foreach(var t in branch)
      //       {
      //         if(!maskOnly || t.Value.IsPointInside(point.Value, double.MinValue, false))
      //         {
      //           indexes.Add(i);
      //           points.Append(point, path);
      //           values.Append(new GH_Number(explorerValues[i]), path);
      //         }
      //       }
      //     }
      //   }
      // }
      // else
      // {
      //   for(var i = 0; i<explorerValues.Length; i++)
      //   {
      //     indexes.Add(i);
      //     points.Append(_explorer.cloud.Points[i].ToGrass());
      //     values.Append(new GH_Number(explorerValues[i]));
      //   }
      // }
      //
      // if(normalizeMask)
      // {
      //   _min = 1.0;
      //   _max = 0.0;
      //   // set the max and min values from the raw input 
      //   foreach(var data in values.FlattenData())
      //     SetMinMax(data.Value);
      // }

    #endregion


      var colors = new List<Color>();
      var values = new List<double>();
      var points = filteredPoints.Values.ToList();

      for(var i = 0; i<points.Count; i++)
      {
        var value = explorerValues[i];
        var gradientColor = _settings.GetColor(value);

        if(settings.showAll || _settings.InRange(value))
        {
          values.Add(value);
          colors.Add(Color.FromArgb(MAX_ALPHA, gradientColor));
        }
      }

      renderedCloud ??= new PointCloud();
      renderedCloud.ClearColors();

      for(var i = 0; i<points.Count; i++)
        renderedCloud.Add(points[i], colors[i]);

      DA.SetDataList(_output.Points, filteredPoints);
      DA.SetDataList(_output.Colors, colors);
      DA.SetDataList(_output.Values, values);

      var activePoint = Math.Min(_settings.point, points.Count-1);

      DA.SetData(_output.ActivePoint, _explorer.cloud.Points[activePoint].ToRhino());
      DA.SetData(_output.ActiveValue, explorerValues[activePoint]);
      DA.SetData(_output.ActiveColor, _settings.GetColor(explorerValues[_settings.point]));

    }

  }

}
