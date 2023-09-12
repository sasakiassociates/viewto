using System.Collections.Generic;
using ViewObjects.Systems.Layouts;

namespace ViewObjects.Speckle
{

  /// <summary>
  ///   Simple Viewer object
  /// </summary>
  public class Layout : ViewObjectBase, ILayout
  {

    /// <summary>
    /// </summary>
    public Layout()
    { }

    /// <summary>
    /// </summary>
    /// <param name="viewers"></param>
    public Layout(List<CubeFace> viewers)
    {
      this.viewers = viewers;
    }

    /// <inheritdoc />
    public List<CubeFace> viewers { get; set; } = new List<CubeFace>();
  }

}
