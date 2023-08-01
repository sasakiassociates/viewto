using System;
using System.Collections.Generic;

namespace ViewObjects.Systems.Layouts
{

  /// <summary>
  /// A simple layout with no presets.By default it uses one viewer in the forward facing direction.
  /// </summary>
  [Serializable]
  public class Layout : ILayout, IViewObject
  {

    public Layout()
    { }

    /// <summary>
    /// Creates a new layout object with the desired viewers passed in with <param name="directions"></param>
    /// </summary>
    /// <param name="directions">List of possible viewers to use with this layout</param>
    public Layout(List<ViewDirection> directions)
    {
      Viewers = directions;
    }

    public List<ViewDirection> Viewers { get; protected set; } =
      new()
        {ViewDirection.Front};
  }

}
