using System.Collections.Generic;
namespace ViewObjects
{
  public class ViewerLinked : IViewerLinked, IViewObject
  {
    public ViewerLinked()
    {
      Layouts = new List<IViewerLayout>();
      Clouds = new List<string>();
    }

    public ViewerLinked(List<IViewerLayout> layouts, List<string> clouds = null)
    {
      Layouts = layouts;
      Clouds = clouds.Valid() ? clouds : new List<string>();
    }

    /// <inheritdoc />
    public bool IsGlobal { get; set; } = true;

    /// <inheritdoc />
    public List<string> Clouds { get; set; }

    /// <inheritdoc />
    public List<IViewerLayout> Layouts { get; set; }
  }
}
