using System;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using ViewObjects;
using ViewObjects.Cloud;
using ViewObjects.Explorer;
using ViewTo.RhinoGh.Goo;
using ViewTo.RhinoGh.Points;
using ViewTo.RhinoGh.Properties;

namespace ViewTo.RhinoGh.Results
{
	public class ResultExplorerComponent : GH_Component
	{
		public ResultExplorerComponent() : base(
			"Result Explorer",
			"EX",
			"Explore a set of view study results",
			ConnectorInfo.CATEGORY,
			ConnectorInfo.Nodes.EXPLORER)
		{
			_explorer = new ResultExplorer();
		}

		PointCloud _pc;
		int _pointSize = 3;
		ResultExplorer _explorer;
		ExplorerSettings _settings;

		double min = 1.0, max = 0.0;

		(int Obj, int Settings, int Mask, int NormalizeByMask, int MaskOnly, int Size) _input;

		protected override void RegisterInputParams(GH_InputParamManager pManager)
		{
			var index = 0;
			pManager.AddGenericParameter("ResultCloud", "R", "ResultCloud for Study", GH_ParamAccess.item);
			_input.Obj = index++;

			pManager.AddGenericParameter("Explorer Settings", "S", "Explorer Input Settings", GH_ParamAccess.item);
			_input.Settings = index++;

			pManager.AddMeshParameter("Mask", "M", "Maskign area to filter points by form", GH_ParamAccess.tree);
			_input.Mask = index++;

			pManager.AddBooleanParameter("Normalize Mask", "NM", "Nomarlize the values by the masked point set", GH_ParamAccess.item, true);
			_input.NormalizeByMask = index++;

			pManager.AddBooleanParameter("Mask Only", "MO", "Show the values only within the mask", GH_ParamAccess.item, true);
			_input.MaskOnly = index++;

			pManager.AddIntegerParameter("Point Size", "P", "Size of Point in Cloud", GH_ParamAccess.item, 3);
			_input.Size = index;

			pManager[_input.NormalizeByMask].Optional = true;
			pManager[_input.MaskOnly].Optional = true;
			pManager[_input.Mask].Optional = true;
			pManager[_input.Size].Optional = true;
		}

		(int Points, int Colors, int Values, int ActivePoint, int ActiveValue, int ActiveColor) _output;

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

		public override void DrawViewportWires(IGH_PreviewArgs args)
		{
			if (_pc != null)
				args.Display.DrawPointCloud(_pc, _pointSize);
		}

		void SetMinMax(double[] value)
		{
			min = 1.0;
			max = 0.0;

			if (value.Valid())
				foreach (var t in value)
					if (!double.IsNaN(t))
						SetMinMax(t);
		}

		void SetMinMax(double value)
		{
			if (value < min) min = value;
			if (value > max) max = value;
		}

		const int MAX_ALPHA = 255;
		const int MIN_ALPHA = 100;

		protected override void SolveInstance(IGH_DataAccess DA)
		{
			DA.GetData(_input.Size, ref _pointSize);

			var normalizeMask = true;
			DA.GetData(_input.NormalizeByMask, ref normalizeMask);

			var maskOnly = true;
			DA.GetData(_input.MaskOnly, ref maskOnly);

			DA.GetDataTree(_input.Mask, out GH_Structure<GH_Mesh> maskTree);

			GH_ObjectWrapper ghWrapper = default;
			DA.GetData(_input.Settings, ref ghWrapper);

			if (ghWrapper?.Value is ExplorerSettings settings)
			{
				_settings = settings;
			}
			else
			{
				AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No Settings passed");
				return;
			}

			GH_ViewObj wrapper = null;
			DA.GetData(_input.Obj, ref wrapper);

			// load cloud point
			if (wrapper?.Value is ResultCloud resultCloud)
			{
				_explorer ??= new ResultExplorer();

				// NOTE: check if this is the same cloud as before
				if (!_explorer.source.Check(resultCloud))
				{
					_explorer.activeType = settings.type;
					_explorer.Load(resultCloud);
				}
			}
			else
			{
				AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
				                  "The cloud being passed in is no bueno! Check the Result Cloud input and reload into the component");
				return;
			}

			if (!_explorer.CheckActiveTarget(_settings.target) || _explorer.activeType != _settings.type)
			{
				// set the active values of the target and type
				_explorer.SetActiveValues(_settings.type, _settings.target);
				// need to store the current max and min of the values passed out
				SetMinMax(_explorer.activeValues);
				// HACK: this is needed to remap the values with power and log. 
				// TODO: this should be replaced once the values are no longer stored as doubles
				_explorer.activeValues = _explorer.activeValues.PowLog(max, max, 10000000.0, 1.0);
			}

			if (!_explorer.activeValues.Valid())
			{
				AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Explorer did not load result cloud properly!");
				return;
			}

			var points = new GH_Structure<GH_Point>();
			var values = new GH_Structure<GH_Number>();

			if (maskTree != null && !maskTree.IsEmpty)
			{
				for (int i = 0; i < _explorer.activeValues.Length; i++)
				{
					var point = _explorer.source.points[i].ToGrass();

					for (var treeIndex = 0; treeIndex < maskTree.Branches.Count; treeIndex++)
					{
						var branch = maskTree.Branches[treeIndex];
						var path = maskTree.Paths[treeIndex];

						foreach (var t in branch)
						{
							if (!maskOnly || t.Value.IsPointInside(point.Value, double.MinValue, false))
							{
								points.Append(point, path);
								values.Append(new GH_Number(_explorer.activeValues[i]), path);
							}
						}
					}
				}
			}
			else
			{
				for (int i = 0; i < _explorer.activeValues.Length; i++)
				{
					points.Append(_explorer.source.points[i].ToGrass());
					values.Append(new GH_Number(_explorer.activeValues[i]));
				}
			}

			if (normalizeMask)
			{
				min = 1.0;
				max = 0.0;
				// set the max and min values from the raw input 
				foreach (var data in values.FlattenData())
					SetMinMax(data.Value);
			}

			var fColors = new GH_Structure<GH_Colour>();
			var fPoints = new GH_Structure<GH_Point>();
			var fValues = new GH_Structure<GH_Number>();

			for (int treeIndex = 0; treeIndex < values.Branches.Count; treeIndex++)
			{
				var path = values.Paths[treeIndex];
				var pointB = points.Branches[treeIndex];
				var valueB = values.Branches[treeIndex];

				for (var leafIndex = 0; leafIndex < valueB.Count; leafIndex++)
				{
					var vIn = valueB[leafIndex].Value;
					var pIn = pointB[leafIndex];

					if (double.IsNaN(vIn))
						vIn = -1;

					// if the value is negative and we are not showing all points, we skip!
					if (vIn <= 0)
					{
						if (!_settings.showAll) continue;

						fPoints.Append(pIn, path);
						fColors.Append(new GH_Colour(_settings.invalidColor), path);
						fValues.Append(new GH_Number(vIn), path);
					}
					else
					{
						// can now normalizing the value related to the masking
						if (normalizeMask)
							vIn = vIn.NormalizeBy(max, min);

						// get the color
						var co = _settings.GetColor(vIn);

						if (_settings.InRange(vIn))
						{
							fPoints.Append(pIn, path);
							fValues.Append(new GH_Number(vIn), path);
							fColors.Append(new GH_Colour(Color.FromArgb(MAX_ALPHA, co)), path);
						}
						else if (_settings.showAll)
						{
							fPoints.Append(pIn, path);
							fValues.Append(new GH_Number(vIn), path);
							fColors.Append(new GH_Colour(Color.FromArgb(MIN_ALPHA, co)), path);
						}
					}
				}
			}

			_pc = new PointCloud();
			_pc.ClearColors();

			var flattenPoints = fPoints.FlattenData();
			var flattenColor = fColors.FlattenData();

			for (var i = 0; i < flattenPoints.Count; i++)
				_pc.Add(flattenPoints[i].Value, flattenColor[i].Value);

			DA.SetDataTree(_output.Points, fPoints);
			DA.SetDataTree(_output.Colors, fColors);
			DA.SetDataTree(_output.Values, fValues);

			DA.SetData(_output.ActivePoint, _explorer.source.points[_settings.point].ToRhino());
			DA.SetData(_output.ActiveValue, _explorer.activeValues[_settings.point]);
			DA.SetData(_output.ActiveColor, _settings.GetColor(_explorer.activeValues[_settings.point]));
		}

		public override Guid ComponentGuid => new Guid("01ffe845-0a7b-4bf8-9d35-48f234fc8cfc");

		protected override Bitmap Icon => new Bitmap(Icons.ExploreResults);

	}
}