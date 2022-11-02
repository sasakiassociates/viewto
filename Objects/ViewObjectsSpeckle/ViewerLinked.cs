using System.Collections.Generic;
namespace ViewObjects.Speckle
{
  /// <summary>
  ///   A linked viewer
  /// </summary>
  public class ViewerLinked : ViewObjectBase, IViewerLinked<Layout>
  {

    /// <summary>
    /// </summary>
    public ViewerLinked()
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="layouts"></param>
    /// <param name="clouds"></param>
    public ViewerLinked(List<Layout> layouts, List<string> clouds)
    {
      Layouts = layouts;
      Clouds = clouds.Valid() ? clouds : new List<string>();
    }

    /// <inheritdoc />
    public List<Layout> Layouts { get; set; } = new List<Layout>();

    /// <inheritdoc />
    public List<string> Clouds { get; set; } = new List<string>();
  }
}
