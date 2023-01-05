namespace ViewObjects.Contents;

/// <summary>
///   basic view content type object
/// </summary>
public interface IContent : IContentInfo
{
  /// <summary>
  ///   The style of view content
  /// </summary>
  public ContentType ContentType { get; }

  /// <summary>
  ///   The color of the content group
  /// </summary>
  public ViewColor Color { get; set; }
}
