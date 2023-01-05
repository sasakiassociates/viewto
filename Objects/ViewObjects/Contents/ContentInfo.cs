using System;
using ViewObjects.Common;

namespace ViewObjects.Contents;

[Serializable]
public class ContentInfo : IContentInfo
{

  public ContentInfo(IContentOption obj)
  {
    ViewName = obj.Name;
    ViewId = obj.Id;
  }

  public ContentInfo(string viewId, string viewName)
  {
    ViewName = viewName;
    ViewId = viewId;
  }

  public ContentInfo(IContent obj)
  {
    ViewName = obj.ViewName;
    ViewId = obj.ViewId;
  }

  /// <inheritdoc />
  public string ViewName { get; set; }

  /// <inheritdoc />
  public string ViewId { get; set; }

  public bool Equals(IContentInfo obj)
  {
    return obj != default(object) && ViewId.Valid() && ViewId.Equals(obj.ViewId);
  }

  public bool Equals(IContentOption obj)
  {
    return obj != default(object) && ViewId.Valid() && ViewId.Equals(obj.Id);
  }

}
