using System.Collections.Generic;

namespace ViewObjects
{

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

	}
}