using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using ViewObjects.Cloud;
using ViewTo.RhinoGh.Goo;

namespace ViewTo.RhinoGh.Results
{
	public class ExtractTargetNames : GH_Component
	{
		public ExtractTargetNames() : base(
			"Extract Targets",
			"ET",
			"Extract Targets from a result cloud into a value list",
			ConnectorInfo.CATEGORY,
			ConnectorInfo.Nodes.RESULTS)
		{ }

		protected override void RegisterInputParams(GH_InputParamManager pManager)
		{
			pManager.AddGenericParameter("Result Cloud", "RC", "Result cloud to get target names from", GH_ParamAccess.item);
			pManager.AddBooleanParameter("Create", "C", "Debugger call for not having the create call go forward", GH_ParamAccess.item);
			pManager.AddBooleanParameter("Update", "U", "Run Update when object created", GH_ParamAccess.item);
			pManager.AddTextParameter("Results", "R", "Results of data", GH_ParamAccess.list);
			pManager[3].Optional = true;
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

		GH_ValueList activeList;
		List<string> storedValues;
		bool create, runUpdate;

		protected override void AfterSolveInstance()
		{
			base.AfterSolveInstance();

			if (!create || !refresh || storedValues == null || !storedValues.Any())
				return;

			var items = CreateValueListItems(storedValues);
			if (activeList == null)
			{
				activeList = PopulateValueList(
					items,
					"Targets",
					"T",
					"A list of view targets"
				);
				var point = new PointF(Attributes.Bounds.Location.X - activeList.Attributes.Bounds.Width,
				                       Attributes.Bounds.Location.Y + (1 * 2 + 1) * Attributes.Bounds.Height / (Params.Input.Count * 2) - 12);
				activeList.Attributes.Pivot = point;
				activeList.SelectItem(0);

				var doc = OnPingDocument();
				doc?.AddObject(activeList, runUpdate);
				Params.Input[3].AddSource(activeList);
			}
			else
			{
				activeList.ListItems.Clear();
				foreach (var i in items)
					activeList.ListItems.Add(i);

				activeList.SelectItem(0);
			}

			refresh = false;
		}

		bool refresh;

		protected override void SolveInstance(IGH_DataAccess DA)
		{
			GH_ViewObj wrapper = null;

			DA.GetData(0, ref wrapper);
			DA.GetData(1, ref create);
			DA.GetData(2, ref runUpdate);

			var inputTargets = new List<string>();
			DA.GetDataList(3, inputTargets);

			if (wrapper?.Value is ResultCloud cloud)
			{
				if (!inputTargets.Any())
				{
					storedValues = cloud.GetTargets();
					refresh = true;
				}
				else
				{
					var tempValues = cloud.GetTargets();
					if (tempValues != null && tempValues.Count != inputTargets.Count)
					{
						storedValues = tempValues;
						refresh = true;
					}
					else
					{
						for (var i = 0; i < inputTargets.Count; i++)
						{
							if (!inputTargets[i].Equals(tempValues[i]))
							{
								storedValues = tempValues;
								refresh = true;
							}
						}
					}
				}
			}

			DA.SetDataList(0, storedValues);
		}

		public override Guid ComponentGuid => new Guid("601505A6-A108-4DB4-AEFA-E15722C008A6");
	}
}