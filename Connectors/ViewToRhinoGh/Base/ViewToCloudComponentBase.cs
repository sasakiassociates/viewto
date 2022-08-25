using Grasshopper.Kernel;
using Rhino.Geometry;

namespace ViewTo.RhinoGh
{
	public abstract class ViewToCloudComponentBase : ViewToComponentBase
	{
		protected Rhino.Geometry.PointCloud renderedCloud;
		protected int pointSize = 3;

		protected ViewToCloudComponentBase(string name, string nickname, string description, string sub) : base(name, nickname, description, sub)
		{
			renderedCloud = new Rhino.Geometry.PointCloud();
		}

		public override void DrawViewportWires(IGH_PreviewArgs args)
		{
			if (renderedCloud != null)
				args.Display.DrawPointCloud(renderedCloud, pointSize);
		}

	}
}