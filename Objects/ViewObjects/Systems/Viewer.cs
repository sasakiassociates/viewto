using System.Collections.Generic;
using ViewObjects.Systems.Layouts;

namespace ViewObjects.Systems;

public class Viewer : IViewer, IViewObject
{

  public Viewer()
  {
    Layouts = new List<ILayout>();
  }

  public Viewer(List<ILayout> layouts)
  {
    Layouts = layouts;
  }

  /// <inheritdoc />
  public List<ILayout> Layouts { get; set; }
}
