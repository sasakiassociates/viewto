using Sasaki.Common;
using Sasaki.Geometry;
using System;
using System.Collections.Generic;

namespace Sasaki.ViewObjects;

public interface ILayout
{
  /// <summary>
  ///   Setup of viewers to use with each layout type
  /// </summary>
  public List<CubeSide> viewers {get;}
}

/// <summary>
/// A simple layout with no presets.By default it uses one viewer in the forward facing direction.
/// </summary>
[Serializable]
public class Layout : ViewObject, ILayout
{

  public Layout()
  { }

  /// <summary>
  /// Creates a new layout object with the desired viewers passed in with <param name="directions"></param>
  /// </summary>
  /// <param name="directions">List of possible viewers to use with this layout</param>
  public Layout(List<CubeSide> directions)
  {
    viewers = directions;
  }

  public List<CubeSide> viewers {get;protected set;} =
    new()
      {CubeSide.Front};
}

/// <summary>
///   Layout with 6 cameras
/// </summary>
public class LayoutCube : Layout
{

  public LayoutCube()
  {
    viewers = new()
    {
      CubeSide.Front,
      CubeSide.Right,
      CubeSide.Back,
      CubeSide.Left,
      CubeSide.Up,
      CubeSide.Down
    };
  }
}

/// <summary>
///   Bundle with singular camera and a point of focus. During analysis this will rotate the camera towards the focus point
/// </summary>
[Serializable]
public class LayoutFocus : Layout
{
  public CloudPoint focus {get;set;}
}

/// <summary>
///   Layout with 4 cameras
/// </summary>
public class LayoutHorizontal : Layout
{
  public LayoutHorizontal()
  {
    viewers = new()
    {
      CubeSide.Front,
      CubeSide.Right,
      CubeSide.Back,
      CubeSide.Left
    };
  }
}

/// <summary>
///   Layout with singular orthographic camera
/// </summary>
[Serializable]
public class LayoutOrtho : Layout
{
  public LayoutOrtho()
  { }

  public LayoutOrtho(List<CubeSide> directions)
  {
    viewers = directions;
  }

  /// <summary>
  /// The orthographic size used for the camera frame. Usually this number relates to half of the viewports bounding box
  /// </summary>
  public double size {get;set;}
}
