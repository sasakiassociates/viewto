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
    public Layout(List<ViewDirection> viewers)
    {
      Viewers = viewers;
    }

    /// <inheritdoc />
    public List<ViewDirection> Viewers { get; set; } = new List<ViewDirection>();
  }

}
