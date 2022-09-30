using System.Collections.Generic;

namespace ViewObjects.Viewer
{
	public class Viewer : IViewerSystem<IViewerLayout>, IViewObject
	{

		public Viewer()
		{
			Layouts = new List<IViewerLayout>();
			Clouds = new List<string>();
		}

		public Viewer(List<IViewerLayout> layouts, List<string> clouds = null)
		{
			Layouts = layouts;
			Clouds = clouds.Valid() ? clouds : new List<string>();
		}

		public List<IViewerLayout> Layouts { get; set; }

		public List<string> Clouds { get; set; }
	}
}