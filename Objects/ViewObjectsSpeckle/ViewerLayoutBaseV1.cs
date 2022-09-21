using System.Collections.Generic;
using Objects.Geometry;
using Speckle.Newtonsoft.Json;

namespace ViewObjects.Speckle
{

	public class ViewerLayoutBaseV1 : ViewObjectBase_v1, IViewerLayout
	{

		public ViewerLayoutBaseV1()
		{ }

		[JsonIgnore] public virtual List<IViewer> viewers =>
			new List<IViewer>
			{
				new Viewer.Viewer(ViewerDirection.Front)
			};
	}

	public class ViewerLayoutBaseV1Focus : ViewerLayoutBaseV1
	{

		public ViewerLayoutBaseV1Focus()
		{ }

		public Point focusPoint { get; set; }
	}

	public class ViewerLayoutBaseV1Normal : ViewerLayoutBaseV1
	{

		public string shellId;

		public ViewerLayoutBaseV1Normal()
		{ }
	}

	public class ViewerLayoutBaseV1Ortho : ViewerLayoutBaseV1
	{

		public double size;

		public ViewerLayoutBaseV1Ortho()
		{ }
	}

	public class ViewerLayoutBaseV1Cube : ViewerLayoutBaseV1
	{

		public ViewerLayoutBaseV1Cube()
		{ }

		[JsonIgnore] public override List<IViewer> viewers =>
			new List<IViewer>
			{
				new Viewer.Viewer(ViewerDirection.Front),
				new Viewer.Viewer(ViewerDirection.Right),
				new Viewer.Viewer(ViewerDirection.Back),
				new Viewer.Viewer(ViewerDirection.Left),
				new Viewer.Viewer(ViewerDirection.Up),
				new Viewer.Viewer(ViewerDirection.Down)
			};
	}

	public class ViewerLayoutBaseV1Horizontal : ViewerLayoutBaseV1
	{

		public ViewerLayoutBaseV1Horizontal()
		{ }

		[JsonIgnore] public override List<IViewer> viewers =>
			new List<IViewer>
			{
				new Viewer.Viewer(ViewerDirection.Front), new Viewer.Viewer(ViewerDirection.Right), new Viewer.Viewer(ViewerDirection.Back),
				new Viewer.Viewer(ViewerDirection.Left)
			};
	}
}