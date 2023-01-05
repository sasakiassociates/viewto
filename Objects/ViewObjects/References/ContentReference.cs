using System;
using System.Collections.Generic;
using ViewObjects.Contents;

namespace ViewObjects.References;

[Serializable]
public class ContentReference : ViewObjectReference<Content>, IContentInfo
{

  /// <inheritdoc />
  public ContentReference(Content obj, List<string> references) : base(obj, references)
  {
    ContentType = obj.ContentType;
  }

  /// <inheritdoc />
  public ContentReference(List<string> references, ContentType type, string viewId, string viewName = null) : base(references, viewId, viewName)
  {
    ContentType = type;
  }

  /// <summary>
  /// </summary>
  public ContentType ContentType { get; }
}
