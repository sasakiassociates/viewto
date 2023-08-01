using System.Collections.Generic;
using ViewObjects.Common;
using ViewObjects.Systems.Layouts;

namespace ViewObjects.Systems
{

  public class ViewerLinked : IViewerLinked, IViewObject
  {
    public ViewerLinked()
    {
      Layouts = new List<ILayout>();
      Clouds = new List<string>();
    }

    public ViewerLinked(List<ILayout> layouts, List<string> clouds = null)
    {
      Layouts = layouts;
      Clouds = clouds.Valid() ? clouds : new List<string>();
    }

    /// <inheritdoc />
    public bool IsGlobal { get; set; } = true;

    /// <inheritdoc />
    public List<string> Clouds { get; set; }

    /// <inheritdoc />
    public List<ILayout> Layouts { get; set; }
  }

}
