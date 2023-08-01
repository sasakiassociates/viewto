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
      this.type = obj.type;
    }

    /// <inheritdoc />
    public ContentReference(List<string> references, ViewContentType type, string viewId, string viewName = null) : base(references, viewId, viewName)
    {
      this.type = type;
    }

    /// <inheritdoc />
    public ViewContentType type { get; }
  }

}
