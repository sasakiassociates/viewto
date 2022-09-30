using System.Collections.Generic;
using ViewObjects;
using ViewTo.Events.Args;

namespace ViewTo.Events.Report
{
	public class PrimedRigArgs : AReportEventArgs
	{
		public readonly Dictionary<string, CloudPoint[]> clouds;
		public readonly List<ViewColor> globalColors;
		public readonly List<IRigParam_v1> globalParams;

		public PrimedRigArgs(List<IRigParam_v1> globalParams, List<ViewColor> globalColors, Dictionary<string, CloudPoint[]> clouds)
		{
			this.globalParams = globalParams;
			this.globalColors = globalColors;
			this.clouds = clouds;
		}

		public PrimedRigArgs(IRig_v1 rigV1)
		{
			if (rigV1 != null)
			{
				clouds = rigV1.clouds;
				globalParams = rigV1.globalParams;
				globalColors = rigV1.globalColors;
			}
		}
	}
}