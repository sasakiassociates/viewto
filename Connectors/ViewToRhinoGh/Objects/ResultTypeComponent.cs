using System;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using ViewObjects;

namespace ViewTo.RhinoGh.Objects
{

	public class ResultTypeComponent : ViewToComponentBase
	{
		public ResultTypeComponent() : base("Result Type",
		                                    "RT",
		                                    "A quick way of selecting a result type",
		                                    ConnectorInfo.Nodes.RESULTS)
		{ }

		public override Guid ComponentGuid => new Guid("4308AD20-1745-41CF-B567-395D15B1E62E");

		private ResultStage _resultStage = ResultStage.Proposed;

		protected override void RegisterInputParams(GH_InputParamManager pManager)
		{ }

		public override bool Write(GH_IWriter writer)
		{
			writer.SetString("resultTypeName", _resultStage.ToString());
			return base.Write(writer);
		}

		public override bool Read(GH_IReader reader)
		{
			var value = "Undefined";
			reader.TryGetString("resultTypeName", ref value);
			_resultStage = (ResultStage)Enum.Parse(typeof(ResultStage), value);

			return base.Read(reader);
		}

		protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
		{
			foreach (ResultStage rt in Enum.GetValues(typeof(ResultStage)))
				Menu_AppendItem(
					menu,
					rt.ToString(), (s, e) =>
					{
						if (s is ToolStripMenuItem item && item.Tag is ResultStage tag)
						{
							_resultStage = tag;
							ExpireSolution(true);
						}
					},
					true,
					rt == _resultStage
				).Tag = rt;

			base.AppendAdditionalComponentMenuItems(menu);
		}

		protected override void RegisterOutputParams(GH_OutputParamManager pManager) =>
			pManager.AddTextParameter("Result Type", "R", "Name of Result Type", GH_ParamAccess.item);

		protected override void SolveInstance(IGH_DataAccess DA) => DA.SetData(0, _resultStage.ToString());
	}

}