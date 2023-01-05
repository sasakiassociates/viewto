using System;
using System.Collections.Generic;

namespace ViewObjects.Systems.Layouts;

/// <summary>
///   Layout with singular orthographic camera
/// </summary>
[Serializable]
public class LayoutOrtho : Layout
{
  public LayoutOrtho()
  { }

  public LayoutOrtho(List<ViewDirection> directions)
  {
    Viewers = directions;
  }

  /// <summary>
  /// The orthographic size used for the camera frame. Usually this number relates to half of the viewports bounding box
  /// </summary>
  public double Size { get; set; }
}
