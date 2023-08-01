using Grasshopper.Kernel;
using Rhino.Geometry;
namespace ViewTo.RhinoGh
{
  public abstract class ViewToCloudComponentBase : ViewToComponentBase
  {
    protected int pointSize = 3;
    protected PointCloud renderedCloud;

    protected ViewToCloudComponentBase(string name, string nickname, string description, string sub) : base(name, nickname, description, sub)
    {
      renderedCloud = new PointCloud();
    }

    public override void DrawViewportWires(IGH_PreviewArgs args)
    {
      if (renderedCloud != null)
      {
        args.Display.DrawPointCloud(renderedCloud, pointSize);
      }
    }
  }
}
