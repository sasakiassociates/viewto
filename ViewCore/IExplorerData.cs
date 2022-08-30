using System.Collections.Generic;
using ViewObjects;

namespace ViewTo
{
	public interface IExplorerData
	{
		public ResultStage ActiveStage { get; set; }

		public List<IResultData> storedData { get; }

		public List<string> targets { get; }
	}
}