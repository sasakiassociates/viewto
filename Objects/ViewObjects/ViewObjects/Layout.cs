using System;
using System.Collections.Generic;

namespace ViewObjects
{

	public class Layout : IViewerLayout, IViewObject
	{

		public Layout()
		{ }

		public Layout(List<ViewDirection> directions)
		{
			Viewers = directions;
		}

		public List<ViewDirection> Viewers { get; protected set; } =
			new List<ViewDirection> { ViewDirection.Front };
	}

	/// <summary>
	///   Layout with singular camera that takes in a cloud with positions and normals
	/// </summary>
	[Serializable]
	public class LayoutNormal : Layout
	{
		public LayoutNormal()
		{ }

		public LayoutNormal(List<ViewDirection> directions)
		{
			Viewers = directions;
		}

		public List<string> Clouds { get; set; }

	}

	/// <summary>
	///   Bundle with singular camera and a point of focus. During analysis this will rotate the camera towards the focus point
	/// </summary>
	[Serializable]
	public class LayoutFocus : Layout
	{

		public LayoutFocus()
		{ }

		public double x { get; set; }

		public double y { get; set; }

		public double z { get; set; }

	}

	/// <summary>
	///   Layout with singular orthographic camera
	/// </summary>
	[Serializable]
	public class LayoutOrtho : Layout
	{
		public LayoutOrtho()
		{ }

		public LayoutOrtho(List<ViewDirection> directions)
		{
			Viewers = directions;
		}

		public double Size { get; set; }
	}

	/// <summary>
	///   Layout with 6 cameras
	/// </summary>
	public class LayoutCube : Layout
	{

		public LayoutCube()
		{
			Viewers = new()
			{
				ViewDirection.Front,
				ViewDirection.Right,
				ViewDirection.Back,
				ViewDirection.Left,
				ViewDirection.Up,
				ViewDirection.Down
			};
		}

	}

	/// <summary>
	///   Layout with 4 cameras
	/// </summary>
	public class LayoutHorizontal : Layout
	{
		public LayoutHorizontal()
		{
			Viewers = new()
			{
				ViewDirection.Front,
				ViewDirection.Right,
				ViewDirection.Back,
				ViewDirection.Left,
			};
		}

	}
}