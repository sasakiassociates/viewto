using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ViewObjects;
using ViewObjects.Explorer;

namespace ViewTo.RhinoGh.Setup
{
	public class CreateContentOption : ViewToComponentBase
	{

		public CreateContentOption() : base("Create View Content Option", "CCO", "Simple node for creating a view content option for exploring results",
		                                    ConnectorInfo.Nodes.RESULTS)
		{ }

		public override Guid ComponentGuid => new Guid("B9AE10FF-B47C-4F71-9A46-7288A5B16624");

		(int Targets, int Stages) _input;

		protected override void RegisterInputParams(GH_InputParamManager pManager)
		{
			var index = 0;
			pManager.AddTextParameter("Targets", "T", "Targets to use", GH_ParamAccess.list);
			_input.Targets = index++;
			pManager.AddTextParameter("Stages", "S", "Stages to use", GH_ParamAccess.list);
			_input.Stages = index;
		}

		protected override void RegisterOutputParams(GH_OutputParamManager pManager)
		{
			pManager.AddGenericParameter("Options", "O", "List of View Content Options", GH_ParamAccess.list);
		}

		protected override void SolveInstance(IGH_DataAccess DA)
		{
			var targets = new List<string>();
			DA.GetDataList(_input.Targets, targets);

			var stages = new List<string>();
			DA.GetDataList(_input.Stages, stages);

			var options = new List<ContentOption>();

			if (stages.Valid() && targets.Valid() && targets.Count == stages.Count)
				for (int i = 0; i < stages.Count; i++)
					options.Add(new ContentOption
					{
						target = targets[i],
						stage = (ResultStage)Enum.Parse(typeof(ResultStage), stages[i])
					});

			if (!options.Valid())
			{
				AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The content options are not valid");
			}

			DA.SetDataList(0, options);
		}
	}
}