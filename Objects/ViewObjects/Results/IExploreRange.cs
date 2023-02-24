using System.Drawing;

namespace ViewObjects.Results
{

  public interface IExploreRange
  {

    public double min { get; }

    public double max { get; }

    public bool normalize { get; }

    public Color[] colorRamp { get; }

    public Color invalidColor { get; }
  }

}
