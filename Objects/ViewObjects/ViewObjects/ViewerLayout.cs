using System;
using System.Collections.Generic;
using ViewObjects.Cloud;

namespace ViewObjects.Viewer
{

	public class ViewerLayout : IViewerLayout, IViewObject
	{

		public ViewerLayout(List<ViewDirection> viewers) => Viewers = viewers;

		public List<ViewDirection> Viewers { get; set; }
	}

	public class ViewerLayoutV1 : IViewerLayout_v1
	{
		public ViewerLayoutV1()
		{ }

		public virtual List<IViewer_v1> viewers
		{
			get =>
				new()
				{
					new ViewerV1(ViewDirection.Front)
				};
		}
	}

	/// <summary>
	///   Layout with singular camera that takes in a cloud with positions and normals
	/// </summary>
	[Serializable]
	public class ViewerLayoutV1Normal : ViewerLayoutV1
	{
		public ViewerLayoutV1Normal()
		{ }

		public CloudShell shell { get; set; }
	}

	/// <summary>
	///   Bundle with singular camera and a point of focus. During analysis this will rotate the camera towards the focus point
	/// </summary>
	[Serializable]
	public class ViewerLayoutV1Focus : ViewerLayoutV1
	{
		public double x { get; set; }

		public double y { get; set; }

		public double z { get; set; }
	}

	/// <summary>
	///   Layout with singular orthographic camera
	/// </summary>
	[Serializable]
	public class ViewerLayoutV1Ortho : ViewerLayoutV1
	{
		public ViewerLayoutV1Ortho()
		{ }

		public double size { get; set; }
	}

	/// <summary>
	///   Layout with 6 cameras
	/// </summary>
	public class ViewerLayoutV1Cube : ViewerLayoutV1
	{

		public ViewerLayoutV1Cube()
		{ }

		public override List<IViewer_v1> viewers
		{
			get =>
				new()
				{
					new ViewerV1(ViewDirection.Front),
					new ViewerV1(ViewDirection.Right),
					new ViewerV1(ViewDirection.Back),
					new ViewerV1(ViewDirection.Left),
					new ViewerV1(ViewDirection.Up),
					new ViewerV1(ViewDirection.Down)
				};
		}
	}

	/// <summary>
	///   Layout with 4 cameras
	/// </summary>
	public class ViewerLayoutV1Horizontal : ViewerLayoutV1
	{
		public override List<IViewer_v1> viewers
		{
			get =>
				new()
				{
					new ViewerV1(ViewDirection.Front), new ViewerV1(ViewDirection.Right), new ViewerV1(ViewDirection.Back),
					new ViewerV1(ViewDirection.Left)
				};
		}
	}

}