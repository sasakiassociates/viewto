using System.Collections.Generic;

namespace ViewObjects.Viewer
{
	public class ViewerSystem : IViewerSystem<IViewerLayout>, IViewObject
	{

		public ViewerSystem()
		{
			Layouts = new List<IViewerLayout>();
			Clouds = new List<string>();
		}

		public ViewerSystem(List<IViewerLayout> layouts, List<string> clouds = null)
		{
			Layouts = layouts;
			Clouds = clouds.Valid() ? clouds : new List<string>();
		}

		public List<IViewerLayout> Layouts { get; set; }

		public List<string> Clouds { get; set; }
	}
}