using System;
using System.Collections.Generic;

namespace ViewObjects
{
	[Serializable]
	public struct ContentResultDataV1 : IResultData_v1
	{
		public ContentResultDataV1(List<int> values, string stage, string content, int color, string meta = null, string layout = null)
		{
			this.values = values;
			this.stage = stage;
			this.content = content;
			this.color = color;
			this.meta = meta;
			this.layout = layout;
		}

		public List<int> values { get; }

		public string content { get; }

		public string meta { get; }

		public int color { get; }

		public string stage { get; }

		public string layout { get; }
	}
}