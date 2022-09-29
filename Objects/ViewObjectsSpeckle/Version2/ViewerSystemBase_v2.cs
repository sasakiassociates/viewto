using System.Collections.Generic;

namespace ViewObjects.Speckle
{
	/// <summary>
	/// 
	/// </summary>
	public class ViewerSystemBase_v2 : ViewObjectBase_v2, IViewerSystem_v2<ViewerLayoutBase_v2>
	{

		/// <inheritdoc />
		public List<ViewerLayoutBase_v2> Layouts { get; set; } = new List<ViewerLayoutBase_v2>();

		/// <inheritdoc />
		public List<string> Clouds { get; set; } = new List<string>();

		/// <summary>
		/// 
		/// </summary>
		public ViewerSystemBase_v2()
		{ }

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
}