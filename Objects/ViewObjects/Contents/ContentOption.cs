using System;
using ViewObjects.Common;

namespace ViewObjects.Contents;

[Serializable]
public class ContentOption : IContentOption
{

  /// <inheritdoc />
  public string Id { get; set; }

  /// <inheritdoc />
  public string Name { get; set; }

  /// <inheritdoc />
  public ContentType Stage { get; set; }

  public bool Equals(IContentOption obj)
  {
    return obj != default(object) && Id.Valid() && Id.Equals(obj.Id) && Stage == obj.Stage;
  }

}
