using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using ViewObjects;
using ViewObjects.Cloud;
using ViewObjects.Explorer;
using ViewTo.RhinoGh.Goo;

namespace ViewTo.RhinoGh.Results
{
	public class GetViewFromValue : ViewToComponentBase
	{

		ResultExplorer _explorer;

		(int Cloud, int Settings, int Values) _input;

		(int Points, int Values, int Colors, int Indexes) _output;

		public GetViewFromValue() : base("Get View From Value", "GVF", "Get a point within a value range", ConnectorInfo.Nodes.EXPLORER) =>
			_explorer = new ResultExplorer();

		public override Guid ComponentGuid
		{
			get => new Guid("B3DC5CEC-D4A4-4769-864B-D6E7D00357AA");
		}

		protected override void RegisterInputParams(GH_InputParamManager pManager)
		{
			var index = 0;

			pManager.AddGenericParameter("Result Cloud", "R", "Result Cloud to use", GH_ParamAccess.item);
			_input.Cloud = index++;

			pManager.AddNumberParameter("Value", "V", "Value to find", GH_ParamAccess.tree);
			_input.Values = index++;

			pManager.AddGenericParameter("Explorer Settings", "S", "Explorer settings to use for filtering", GH_ParamAccess.item);
			_input.Settings = index;
		}

		protected override void RegisterOutputParams(GH_OutputParamManager pManager)
		{
			var index = 0;

			pManager.AddNumberParameter("Values", "V", "Values that were located", GH_ParamAccess.tree);
			_output.Values = index++;

			pManager.AddPointParameter("Points", "P", "Points that were located", GH_ParamAccess.tree);
			_output.Points = index++;

			pManager.AddColourParameter("Colors", "C", "Colors that were located", GH_ParamAccess.tree);
			_output.Colors = index++;

			pManager.AddIntegerParameter("Indexes", "I", "Indexes that were located", GH_ParamAccess.tree);
			_output.Indexes = index;
		}

		protected override void SolveInstance(IGH_DataAccess DA)
		{
			var obj = new GH_ViewObj();
			DA.GetData(_input.Cloud, ref obj);

			ResultCloudV1V1 cloud = null;
			if (obj?.Value is ResultCloudV1V1 rc)
				cloud = rc;
			else
				return;

			GH_ObjectWrapper ghWrapper = default;
			DA.GetData(_input.Settings, ref ghWrapper);

			ExplorerSettings settings = default;

			if (ghWrapper?.Value is ExplorerSettings value)
			{
				settings = value;
			}
			else
			{
				AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No Settings passed");
				return;
			}

			DA.GetDataTree(_input.Values, out GH_Structure<GH_Number> valueTree);

			_explorer ??= new ResultExplorer();

			// // NOTE: check if this is the same cloud as before
			// if (!_explorer.source.Check(cloud))
			// 	_explorer.Load(cloud);

			// if there is only one option we only need to check once
			if ((settings.options.Valid(1) && !_explorer.CheckActiveTarget(settings.options[0].Name)) || _explorer.activeStage != settings.options[0].Stage)
				_explorer.SetActiveValues(settings.options[0].Stage, settings.options[0].Name);

			var o_indexTree = new GH_Structure<GH_Integer>();
			var o_valueTree = new GH_Structure<GH_Number>();
			var o_pointTree = new GH_Structure<GH_Point>();
			var o_colorTree = new GH_Structure<GH_Colour>();

			// for (var i = 0; i < valueTree.Branches.Count; i++)
			// {
			// 	var branch = valueTree.Branches[i];
			// 	var path = valueTree.Paths[i];
			// 	foreach (var v in branch)
			// 	{
			// 		var indexResult = _explorer.FindPointWithValue(v.Value);
			// 		var pointValue = _explorer.activeValues[indexResult];
			//
			// 		o_indexTree.Append(new GH_Integer(indexResult), path);
			// 		o_valueTree.Append(new GH_Number(pointValue), path);
			// 		o_pointTree.Append(_explorer.source.points[indexResult].ToGrass(), path);
			// 		o_colorTree.Append(new GH_Colour(settings.GetColor(pointValue)), path);
			// 	}
			// }
			//
			// DA.SetDataTree(_output.Indexes, o_indexTree);
			// DA.SetDataTree(_output.Values, o_valueTree);
			// DA.SetDataTree(_output.Points, o_pointTree);
			// DA.SetDataTree(_output.Colors, o_colorTree);
		}
	}
}