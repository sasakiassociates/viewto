using System.Collections.Generic;

namespace ViewObjects.Speckle
{

	/// <summary>
	/// Object for grouping of view content
	/// </summary>
	public class ContentsBase : ViewObjectBase_v2, IContents<ContentBase_v2>
	{
		/// <summary>
		/// List of view content objects
		/// </summary>
		public List<ContentBase_v2> Contents { get; set; }
	}

}