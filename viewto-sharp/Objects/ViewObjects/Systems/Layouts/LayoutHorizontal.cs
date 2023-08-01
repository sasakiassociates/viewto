namespace ViewObjects.Systems.Layouts
{

  /// <summary>
  ///   Layout with 4 cameras
  /// </summary>
  public class LayoutHorizontal : Layout
  {
    public LayoutHorizontal()
    {
      Viewers = new()
      {
        ViewDirection.Front,
        ViewDirection.Right,
        ViewDirection.Back,
        ViewDirection.Left
      };
    }
  }

}
