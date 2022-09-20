using System.Collections.Generic;
using Speckle.Core.Models;

namespace ViewObjects.Speckle
{

	/// <summary>
	/// Object for grouping of view content
	/// </summary>
	public class ContentsBase : ViewObjectBase, IContents<ContentBase_v2>
	{
		/// <summary>
		/// List of view content objects
		/// </summary>
		public List<ContentBase_v2> Contents { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class ResultCloudDataBase : ViewObjectBase, IResultCloudData
	{
		/// <summary>
		/// 
		/// </summary>
		public string ContentId { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public ResultStage Stage { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string Layout { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[Chunkable] public int[] Values { get; set; }
	}

}