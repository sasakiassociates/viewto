namespace ViewObjects.Contents;

public interface IContentOption
{
  /// <summary>
  ///   Id linked to <see cref="IContent" />
  /// </summary>
  public string Id { get; set; }

  /// <summary>
  ///   Name of the Target Content
  /// </summary>
  public string Name { get; set; }

  /// <summary>
  ///   the stage to use for
  /// </summary>
  public ContentType Stage { get; set; }
}
