namespace ViewObjects.Systems.Layouts;

/// <summary>
///   Layout with 6 cameras
/// </summary>
public class LayoutCube : Layout
{

  public LayoutCube()
  {
    Viewers = new()
    {
      ViewDirection.Front,
      ViewDirection.Right,
      ViewDirection.Back,
      ViewDirection.Left,
      ViewDirection.Up,
      ViewDirection.Down
    };
  }
}
