using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ViewObjects;
using ViewObjects.Contents;
using ViewObjects.Results;

namespace ViewTo.RhinoGh.Results
{

  public class ExplorerSettingsComponent : ViewToComponentBase
  {

    ( int ValueType, int Normalize, int Range, int Show, int Point, int Colors, int InvalidColor) _input;

    public ExplorerSettingsComponent()
      : base("Explorer Settings",
        "ES",
        "Result Explorer Settings",
        ConnectorInfo.Nodes.EXPLORER)
    { }

    public override Guid ComponentGuid => new Guid("4817C001-C72E-4992-AF53-5CDB16D55765");

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      var index = 0;

      pManager.AddTextParameter("Value Type", "V", "Explorer Value type to compare with", GH_ParamAccess.item);
      _input.ValueType = index++;

      pManager.AddIntervalParameter("Range", "R", "Value Range of pixels to show", GH_ParamAccess.item, new Interval(0, 1));
      _input.Range = index++;

      pManager.AddIntegerParameter("Index", "I", "Active Point to set use", GH_ParamAccess.item, 0);
      _input.Point = index++;

      pManager.AddBooleanParameter("Show", "S", "Option to show points with values only", GH_ParamAccess.item, true);
      _input.Show = index++;

      pManager.AddBooleanParameter("Normalize", "N", "Normalize the active values", GH_ParamAccess.item, false);
      _input.Normalize = index++;

      pManager.AddColourParameter("Colors", "C", "Ordered list of colors for use with display.", GH_ParamAccess.list);
      _input.Colors = index++;

      pManager.AddColourParameter("Empty", "E", "Color to use for any value that's emtpty.", GH_ParamAccess.item);
      _input.InvalidColor = index;

      pManager[_input.ValueType].Optional = true;
      pManager[_input.Range].Optional = true;
      pManager[_input.Show].Optional = true;
      pManager[_input.Normalize].Optional = true;
      pManager[_input.Point].Optional = true;
      pManager[_input.Colors].Optional = true;
      pManager[_input.InvalidColor].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Settings", "S", "Settings for filtering result data", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      var valueType = ExplorerValueType.ExistingOverPotential.ToString();
      DA.GetData(_input.ValueType, ref valueType);

      var normalize = false;
      DA.GetData(_input.Normalize, ref normalize);

      var show = false;
      DA.GetData(_input.Show, ref show);

      var index = 0;
      DA.GetData(_input.Point, ref index);

      Interval range = default;
      DA.GetData(_input.Range, ref range);

      Color invalidColor = default;
      DA.GetData(_input.InvalidColor, ref invalidColor);

      var colors = new List<Color>();
      if(!DA.GetDataList(_input.Colors, colors))
        colors = ViewColor.Ramp().ToList();

      var settings = new ExplorerSettings
      {
        point = Math.Max(index, 0),
        min = range.Min,
        max = range.Max,
        colorRamp = colors.ToArray(),
        invalidColor = invalidColor,
        showAll = show,
        normalize = normalize,
        valueType = (ExplorerValueType)Enum.Parse(typeof(ExplorerValueType), valueType)
      };

      DA.SetData(0, settings);
    }
  }

}
