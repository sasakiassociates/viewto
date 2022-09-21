using System.Collections.Generic;
using Speckle.Newtonsoft.Json;

namespace ViewObjects.Speckle
{

	public class ResultCloudBaseV1 : ViewCloudBaseV1
	{

		public ResultCloudBaseV1()
		{ }

		[JsonIgnore] public override bool isValid => base.isValid && data.Valid();

		public List<IResultData> data { get; set; }
	}
}