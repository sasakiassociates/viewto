using System;

namespace ViewObjects.Viewer
{
	[Serializable]
	public class ViewerV1 : IViewer_v1
	{
		// Empty constructor for serializing 
		public ViewerV1()
		{ }

		public ViewerV1(ViewerDirection direction) => Direction = direction;

		public ViewerDirection Direction { get; }
	}
}