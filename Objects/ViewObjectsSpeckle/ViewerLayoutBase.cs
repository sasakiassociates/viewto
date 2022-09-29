using System.Collections.Generic;

namespace ViewObjects.Speckle
{

	/// <summary>
	///   Simple Viewer object
	/// </summary>
	public class ViewerLayoutBase : ViewObjectBase, IViewerLayout_v2
	{

		/// <summary>
		/// </summary>
		public ViewerLayoutBase()
		{ }

		/// <summary>
		/// </summary>
		/// <param name="viewers"></param>
		public ViewerLayoutBase(List<ViewerDirection> viewers) => Viewers = viewers;

		/// <inheritdoc />
		public List<ViewerDirection> Viewers { get; set; } = new List<ViewerDirection>();
	}
}