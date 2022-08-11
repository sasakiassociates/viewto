using System;
using System.Collections.Generic;
using System.Linq;

namespace ViewObjects.Cloud
{
	public class ResultCloud : ViewCloud, IResultCloud
	{
		public ResultCloud() => viewID = Guid.NewGuid().ToString();

		public override bool isValid
		{
			get => base.isValid && data != null && data.Any();
		}

		public List<IResultData> data { get; set; }
	}
}