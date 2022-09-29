using System.Collections.Generic;

namespace ViewObjects.Speckle
{
	/// <summary>
	/// </summary>
	public class ViewerSystemBase : ViewObjectBase, IViewerSystem_v2<ViewerLayoutBase>
	{

		/// <summary>
		/// </summary>
		public ViewerSystemBase()
		{ }

		/// <summary>
		/// </summary>
		/// <param name="layouts"></param>
		/// <param name="clouds"></param>
		public ViewerSystemBase(List<ViewerLayoutBase> layouts, List<string> clouds)
		{
			Layouts = layouts;
			Clouds = clouds.Valid() ? clouds : new List<string>();
		}

		/// <inheritdoc />
		public List<ViewerLayoutBase> Layouts { get; set; } = new List<ViewerLayoutBase>();

		/// <inheritdoc />
		public List<string> Clouds { get; set; } = new List<string>();
	}
}