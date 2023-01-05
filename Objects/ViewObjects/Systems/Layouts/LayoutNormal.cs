using System;
using System.Collections.Generic;

namespace ViewObjects.Systems.Layouts;

/// <summary>
///   Layout with singular camera that takes in a cloud with positions and normals
/// </summary>
[Serializable]
public class LayoutNormal : Layout
{
  public LayoutNormal()
  { }

  public LayoutNormal(List<ViewDirection> directions)
  {
    Viewers = directions;
  }

  public List<string> Clouds { get; set; }
}
