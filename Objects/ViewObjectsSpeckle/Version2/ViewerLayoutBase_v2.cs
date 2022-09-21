using System.Collections.Generic;

namespace ViewObjects.Speckle
{

	/// <summary>
	/// 
	/// </summary>
	public class ViewerSystemBase_v2 : ViewObjectBase_v2, IViewerSystem_v2<ViewerLayoutBase_v2>
	{

		/// <inheritdoc />
		public List<ViewerLayoutBase_v2> Layouts { get; set; }

		/// <inheritdoc />
		public List<string> Clouds { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public ViewerSystemBase_v2()
		{
			this.Layouts = new List<ViewerLayoutBase_v2>();
			this.Clouds = new List<string>();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="layouts"></param>
		/// <param name="clouds"></param>
		public ViewerSystemBase_v2(List<ViewerLayoutBase_v2> layouts, List<string> clouds)
		{
			this.Layouts = layouts;
			this.Clouds = clouds.Valid() ? clouds : new List<string>();
		}
	}

	/// <summary>
	/// Simple Viewer object
	/// </summary>
	public class ViewerLayoutBase_v2 : ViewObjectBase_v2, IViewerLayout_v2
	{
		/// <inheritdoc />
		public List<ViewerDirection> Viewers { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public ViewerLayoutBase_v2()
		{
			Viewers = new List<ViewerDirection>();
		}

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