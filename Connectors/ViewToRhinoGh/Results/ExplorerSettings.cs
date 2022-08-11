using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;
using ViewObjects;
using ViewObjects.Explorer;

namespace ViewTo.RhinoGh.Results
{

	public class ExplorerSettingsComponent : GH_Component
	{
		public ExplorerSettingsComponent() : base(
			"Explorer Settings",
			"ES",
			"Result Explorer Settings",
			ConnectorInfo.CATEGORY,
			ConnectorInfo.Nodes.EXPLORER
		)
		{ }

		public override Guid ComponentGuid => new Guid("4817C001-C72E-4992-AF53-5CDB16D55765");

		(int Target, int Stage, int Range, int Show, int Point, int Colors, int InvalidColor) _input;

		protected override void RegisterInputParams(GH_InputParamManager pManager)
		{
			var index = 0;

			pManager.AddTextParameter("View Target", "T", "Target content to use", GH_ParamAccess.item);
			_input.Target = index++;

			pManager.AddTextParameter("Stage", "S", "Stage for pixel data", GH_ParamAccess.item);
			_input.Stage = index++;

			pManager.AddIntervalParameter("Range", "R", "Value Range of pixels to show", GH_ParamAccess.item, new Interval(0, 1));
			_input.Range = index++;

			pManager.AddIntegerParameter("Index", "I", "Active Point to set use", GH_ParamAccess.item, -1);
			_input.Point = index++;

			pManager.AddBooleanParameter("Show", "S", "Option to show points with values only", GH_ParamAccess.item, true);
			_input.Show = index++;

			pManager.AddColourParameter("Colors", "C", "Ordered list of colors for use with display.", GH_ParamAccess.list);
			_input.Colors = index++;

			pManager.AddColourParameter("Empty", "E", "Color to use for any value that's emtpty.", GH_ParamAccess.item);
			_input.InvalidColor = index;

			pManager[_input.Range].Optional = true;
			pManager[_input.Show].Optional = true;
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
			var target = string.Empty;
			DA.GetData(_input.Target, ref target);

			var stage = string.Empty;
			DA.GetData(_input.Stage, ref stage);

			var show = false;
			DA.GetData(_input.Show, ref show);

			var index = -1;
			DA.GetData(_input.Point, ref index);

			Interval range = default;
			DA.GetData(_input.Range, ref range);

			Color invalidColor = default;
			DA.GetData(_input.InvalidColor, ref invalidColor);

			var colors = new List<Color>();
			if (!DA.GetDataList(_input.Colors, colors))
				colors = Commander.BasicColorRamp.ToList();
			
			var settings = new ExplorerSettings
			{
				target = target,
				type = (ResultType)Enum.Parse(typeof(ResultType), stage),
				point = index,
				min = range.Min,
				max = range.Max,
				ramp = colors.ToArray(),
				invalid = invalidColor,
				showAll = show,
			};

			DA.SetData(0, settings);
		}
	}
}