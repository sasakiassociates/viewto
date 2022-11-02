using System.Collections.Generic;
namespace ViewObjects.Speckle
{

  /// <summary>
  /// </summary>
  public class Viewer : ViewObjectBase, IViewer<Layout>
  {

    /// <summary>
    /// </summary>
    public Viewer()
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="layouts"></param>
    public Viewer(List<Layout> layouts)
    {
      Layouts = layouts;
    }

    /// <inheritdoc />
    public List<Layout> Layouts { get; set; } = new List<Layout>();
  }
}
