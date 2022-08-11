using System;

namespace ViewObjects.Viewer
{
	[Serializable]
	public class Viewer : IViewer
	{
		// Empty constructor for serializing 
		public Viewer()
		{ }

		public Viewer(ViewerDirection direction) => Direction = direction;

		public ViewerDirection Direction { get; }
	}
}