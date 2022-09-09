using System.Collections.Generic;
using System.Linq;
using ViewObjects;

namespace ViewTo
{

	public class ResultExplorer : IResultExplorer
	{

		public IResultCloud source { get; private set; }

		public List<string> targets { get; private set; }

		public List<IResultData> storedData { get; private set; }

		public ResultStage activeStage { get; set; } = ResultStage.Existing;

		public string activeTarget { get; set; }

		public int[] activeValues { get; set; }

		public int activePoint { get; set; }

		public void Load(IResultCloud obj)
		{
			if (obj == default)
				return;

			source = obj;
			storedData = source.data;
			targets = source.GetTargets();
			activeTarget = targets.FirstOrDefault();

			activePoint = 0;
			// set default value
			activeValues = this.Fetch(activeStage).ToArray();
		}
	}
}