using System.Collections.Generic;
using ViewObjects;

namespace ViewTo
{
	public interface IExplorerData
	{
		public ResultStage activeStage { get; set; }

		public List<IResultData_v1> storedData { get; }

		public List<string> targets { get; }
	}
}