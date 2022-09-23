using System.Collections.Generic;
using Speckle.Core.Models;

namespace ViewObjects.Speckle
{

	public class ResultPixelBaseV1 : ViewObjectBase_v1, IResultData
	{

		[DetachProperty] [Chunkable] public List<int> values { get; set; } = new List<int>();

		public string content { get; set; }

		public string stage { get; set; }

		public string meta { get; set; }

		public int color { get; set; }

		public string layout { get; set; }

		public ResultPixelBaseV1()
		{ }
	}
}