using System.Collections.Generic;
using Speckle.Core.Models;

namespace ViewObjects.Speckle
{
	/// <summary>
	/// 
	/// </summary>
	public class ResultCloudBase_v2 : ViewObjectBase_v2
	{

		/// <summary>
		/// List of point positions as x,y,z
		/// </summary>
		[DetachProperty] [Chunkable(31250)] public List<double> Points { get; set; } = new List<double>();

		/// <summary>
		/// 
		/// </summary>
		[DetachProperty] public List<IResultCloudData> Data { get; set; } = new List<IResultCloudData>();

		/// <summary>
		/// 
		/// </summary>
		public ResultCloudBase_v2()
		{ }

	}
}