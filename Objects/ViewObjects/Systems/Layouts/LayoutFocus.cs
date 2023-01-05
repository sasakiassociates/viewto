using System;

namespace ViewObjects.Systems.Layouts;

/// <summary>
///   Bundle with singular camera and a point of focus. During analysis this will rotate the camera towards the focus point
/// </summary>
[Serializable]
public class LayoutFocus : Layout
{

  public LayoutFocus()
  { }

  public double x { get; set; }

  public double y { get; set; }

  public double z { get; set; }
}
