using System.Collections.Generic;

namespace ViewObjects.Viewer
{
	public class Viewer : IViewerSystem, IViewObject
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

		/// <inheritdoc />
		public List<IViewerLayout> Layouts { get; set; }

		/// <inheritdoc />
		public List<string> Clouds { get; set; }

		/// <inheritdoc />
		public bool IsGlobal { get; set; } = true;

		/// <inheritdoc />
		public List<IContent> Contents { get; set; }
	}
}