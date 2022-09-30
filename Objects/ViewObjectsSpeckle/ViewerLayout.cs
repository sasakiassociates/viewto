using System.Collections.Generic;

namespace ViewObjects.Speckle
{

	/// <summary>
	///   Simple Viewer object
	/// </summary>
	public class ViewerLayout : ViewObjectBase, IViewerLayout
	{

		/// <summary>
		/// </summary>
		public ViewerLayout()
		{ }

		/// <summary>
		/// </summary>
		/// <param name="viewers"></param>
		public ViewerLayout(List<ViewerDirection> viewers)
		{
			Viewers = viewers;
		}

		/// <inheritdoc />
		public List<ViewerDirection> Viewers { get; set; } = new List<ViewerDirection>();

	}
}