using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using ViewObjects.Cloud;
using ViewTo.RhinoGh.Goo;

namespace ViewTo.RhinoGh.Results
{
	public class ExtractTargetNames : ViewToComponentBase
	{

		bool _refresh;
		GH_ValueList _activeList;
		List<string> _storedValues;

		public ExtractTargetNames() : base(
			"Extract Targets",
			"ET",
			"Extract Targets from a result cloud into a value list",
			ConnectorInfo.Nodes.RESULTS)
		{ }

		protected override void RegisterInputParams(GH_InputParamManager pManager)
		{
			pManager.AddGenericParameter("Result Cloud", "RC", "Result cloud to get target names from", GH_ParamAccess.item);
			pManager.AddTextParameter("Results", "R", "Results of data", GH_ParamAccess.list);
			pManager[1].Optional = true;
		}

		protected override void RegisterOutputParams(GH_OutputParamManager pManager)
		{
			pManager.AddTextParameter("Targets", "T", "List of View Targets", GH_ParamAccess.list);
		}

		public static List<GH_ValueListItem> CreateValueListItems(List<string> values, List<string> expressions = null)
		{
			var items = new List<GH_ValueListItem>();
			if (values != null && values.Any())
			{
				var useExpressions = expressions != null && expressions.Count == values.Count;

				for (var i = 0; i < values.Count; i++)
					items.Add(new GH_ValueListItem(values[i], useExpressions ? $"\"{expressions[i]}\"" : $"\"{values[i]}\""));
			}

			return items;
		}

		public static GH_ValueList PopulateValueList(List<GH_ValueListItem> values, string name, string nickName, string description)
		{
			GH_ValueList valueList = null;
			//make dropdown box
			if (values != null && values.Any())
			{
				valueList = new GH_ValueList();
				valueList.CreateAttributes();
				valueList.Name = name;
				valueList.NickName = nickName;
				valueList.Description = description;
				valueList.ListMode = GH_ValueListMode.DropDown;

				valueList.ListItems.Clear();

				foreach (var t in values)
					valueList.ListItems.Add(t);
			}

			return valueList;
		}

		protected override void AfterSolveInstance()
		{
			base.AfterSolveInstance();

			if (!_refresh || _storedValues == null || !_storedValues.Any())
				return;

			var items = CreateValueListItems(_storedValues);
			if (_activeList == null)
			{
				_activeList = PopulateValueList(
					items,
					"Targets",
					"T",
					"A list of view targets"
				);
				var point = new PointF(Attributes.Bounds.Location.X - _activeList.Attributes.Bounds.Width,
				                       Attributes.Bounds.Location.Y + (1 * 2 + 1) * Attributes.Bounds.Height / (Params.Input.Count * 2) - 12);
				_activeList.Attributes.Pivot = point;
				_activeList.SelectItem(0);

				var doc = OnPingDocument();
				doc?.AddObject(_activeList, true);
				Params.Input[3].AddSource(_activeList);
			}
			else
			{
				_activeList.ListItems.Clear();
				foreach (var i in items)
					_activeList.ListItems.Add(i);

				_activeList.SelectItem(0);
			}

			_refresh = false;
		}

		protected override void SolveInstance(IGH_DataAccess DA)
		{
			
			var graphMapper = new GH_GraphMapper();
			var graphMapperAttributes = new GH_GraphMapperAttributes(graphMapper);
			
			GH_ViewObj wrapper = null;
			DA.GetData(0, ref wrapper);

			var inputTargets = new List<string>();
			DA.GetDataList(1, inputTargets);

			if (wrapper?.Value is ResultCloud cloud)
			{
				if (!inputTargets.Any())
				{
					_storedValues = cloud.GetTargets();
					_refresh = true;
				}
				else
				{
					var tempValues = cloud.GetTargets();
					if (tempValues != null && tempValues.Count != inputTargets.Count)
					{
						_storedValues = tempValues;
						_refresh = true;
					}
					else
					{
						for (var i = 0; i < inputTargets.Count; i++)
						{
							if (!inputTargets[i].Equals(tempValues[i]))
							{
								_storedValues = tempValues;
								_refresh = true;
							}
						}
					}
				}
			}

			DA.SetDataList(0, _storedValues);
		}

		public override Guid ComponentGuid => new Guid("601505A6-A108-4DB4-AEFA-E15722C008A6");
	}
}