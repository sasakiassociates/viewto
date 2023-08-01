namespace ViewObjects.Contents
{

  /// <summary>
  ///   basic view content type object
  /// </summary>
  public interface IContent : IContentInfo
  {
    /// <summary>
    ///   The color of the content group
    /// </summary>
    public ViewColor Color { get; set; }
  }

}
