using System;
using System.Collections.Generic;
using System.Linq;

namespace ViewObjects.Cloud
{
	public class ResultCloudV1V1 : ViewCloudV1V1, IResultCloudV1
	{
		public ResultCloudV1V1() => ViewId = Guid.NewGuid().ToString();

		public override bool IsValid
		{
			get => base.IsValid && data != null && data.Any();
		}

		public List<IResultData_v1> data { get; set; }
	}

}