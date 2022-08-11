using System;
using System.Collections.Generic;
using ViewObjects.Cloud;

namespace ViewObjects.Viewer
{

	public class ViewerLayout : IViewerLayout
	{
		public ViewerLayout()
		{ }

		public virtual List<IViewer> viewers
		{
			get =>
				new List<IViewer>
				{
					new Viewer(ViewerDirection.Front)
				};
		}
	}

	/// <summary>
	///   Layout with singular camera that takes in a cloud with positions and normals
	/// </summary>
	[Serializable]
	public class ViewerLayoutNormal : ViewerLayout
	{
		public ViewerLayoutNormal()
		{ }

		public CloudShell shell { get; set; }
	}

	/// <summary>
	///   Bundle with singular camera and a point of focus. During analysis this will rotate the camera towards the focus point
	/// </summary>
	[Serializable]
	public class ViewerLayoutFocus : ViewerLayout
	{
		public double x { get; set; }
		public double y { get; set; }
		public double z { get; set; }
	}

	/// <summary>
	///   Layout with singular orthographic camera
	/// </summary>
	[Serializable]
	public class ViewerLayoutOrtho : ViewerLayout
	{
		public ViewerLayoutOrtho()
		{ }

		public double size { get; set; }
	}

	/// <summary>
	///   Layout with 6 cameras
	/// </summary>
	public class ViewerLayoutCube : ViewerLayout
	{

		public ViewerLayoutCube()
		{ }

		public override List<IViewer> viewers
		{
			get =>
				new List<IViewer>
				{
					new Viewer(ViewerDirection.Front),
					new Viewer(ViewerDirection.Right),
					new Viewer(ViewerDirection.Back),
					new Viewer(ViewerDirection.Left),
					new Viewer(ViewerDirection.Up),
					new Viewer(ViewerDirection.Down)
				};
		}
	}

	/// <summary>
	///   Layout with 4 cameras
	/// </summary>
	public class ViewerLayoutHorizontal : ViewerLayout
	{
		public override List<IViewer> viewers
		{
			get =>
				new List<IViewer>
				{
					new Viewer(ViewerDirection.Front), new Viewer(ViewerDirection.Right), new Viewer(ViewerDirection.Back), new Viewer(ViewerDirection.Left)
				};
		}
	}

}