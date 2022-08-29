using System.Collections.Generic;
using ViewObjects.Content;

namespace ViewObjects.Unity
{
	public class ResultsForCloud
	{
		public ResultsForCloud(string id, List<IResultData> data)
		{
			this.id = id;
			this.data = data;
		}

		public string id
		{
			get;
		}

		public List<IResultData> data
		{
			get;
		}
	}
}