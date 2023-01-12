using System.Collections.Generic;
using ViewObjects.Clouds;

namespace ViewObjects.Unity
{
	public class ResultsForCloud
	{
		public ResultsForCloud(string id, List<IResultCloudData> data)
		{
			this.id = id;
			this.data = data;
		}

		public string id { get; }

		public List<IResultCloudData> data { get; }
	}
}