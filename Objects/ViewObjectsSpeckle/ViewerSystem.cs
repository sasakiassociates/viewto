using System.Collections.Generic;

namespace ViewObjects.Speckle
{
	/// <summary>
	/// </summary>
	public class ViewerSystem : ViewObjectBase, IViewerSystem<ViewerLayout>
	{

		/// <summary>
		/// </summary>
		public ViewerSystem()
		{ }

		/// <summary>
		/// </summary>
		/// <param name="layouts"></param>
		/// <param name="clouds"></param>
		public ViewerSystem(List<ViewerLayout> layouts, List<string> clouds)
		{
			Layouts = layouts;
			Clouds = clouds.Valid() ? clouds : new List<string>();
		}

		/// <inheritdoc />
		public List<ViewerLayout> Layouts { get; set; } = new List<ViewerLayout>();

		/// <inheritdoc />
		public List<string> Clouds { get; set; } = new List<string>();
	}
}