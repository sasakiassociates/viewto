using System;
using System.Collections.Generic;
using ViewObjects.Contents;

namespace ViewObjects.References
{

  [Serializable]
  public class ContentReference : ViewObjectReference<Content>, IContentInfo
  {

    /// <inheritdoc />
    public ContentReference(Content obj, List<string> references) : base(obj, references)
    {
      this.contentType = obj.contentType;
    }

    /// <inheritdoc />
    public ContentReference(List<string> references, ViewContentType type, string viewId, string viewName = null) : base(references, viewId, viewName)
    {
      this.contentType = type;
    }

    /// <inheritdoc />
    public ViewContentType contentType { get; }
  }

}
