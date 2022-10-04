using System.Collections.Generic;

namespace ViewObjects.Viewer
{

	public class ViewerLinked : IViewerLinked, IViewObject
	{
		public ViewerLinked()
		{
			Layouts = new List<IViewerLayout>();
			Clouds = new List<string>();
		}

		public ViewerLinked(List<IViewerLayout> layouts, List<string> clouds = null)
		{
			Layouts = layouts;
			Clouds = clouds.Valid() ? clouds : new List<string>();
		}

		/// <inheritdoc />
		public List<string> Clouds { get; set; }

		/// <inheritdoc />
		public List<IViewerLayout> Layouts { get; set; }

		/// <inheritdoc />
		public bool IsGlobal { get; set; } = true;

	}

	public class Viewer : IViewer, IViewObject
	{

		public Viewer()
		{
			Layouts = new List<IViewerLayout>();
		}

		public Viewer(List<IViewerLayout> layouts)
		{
			Layouts = layouts;
		}

		/// <inheritdoc />
		public List<IViewerLayout> Layouts { get; set; }

		/// <inheritdoc />
		public bool IsGlobal { get; set; } = true;

	}
}