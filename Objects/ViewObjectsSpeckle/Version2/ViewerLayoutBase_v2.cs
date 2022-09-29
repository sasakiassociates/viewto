using System.Collections.Generic;

namespace ViewObjects.Speckle
{

	/// <summary>
	/// Simple Viewer object
	/// </summary>
	public class ViewerLayoutBase_v2 : ViewObjectBase_v2, IViewerLayout_v2
	{
		/// <inheritdoc />
		public List<ViewerDirection> Viewers { get; set; } = new List<ViewerDirection>();

		/// <summary>
		/// 
		/// </summary>
		public ViewerLayoutBase_v2()
		{ }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="viewers"></param>
		public ViewerLayoutBase_v2(List<ViewerDirection> viewers)
		{
			this.Viewers = viewers;
		}

	}
}