using System;
using Sasaki.Common;

namespace ViewObjects.Contents
{

  [Serializable]
  public class ContentInfo : IContentInfo
  {

    public ContentInfo()
    { }


    public ContentInfo(string viewId, string viewName)
    {
      name = viewName;
      appId = viewId;
    }

    public ContentInfo(IContent obj)
    {
      name = obj.name;
      appId = obj.appId;
      contentType = obj.contentType;
    }


    /// <inheritdoc />
    public string name { get; set; }

    public ViewContentType contentType { get; set; }


    /// <inheritdoc />
    public string appId { get; set; }

    public bool Equals(IContentInfo obj)
    {
      return obj != default(object) && appId.Valid() && appId.Equals(obj.appId);
    }


  }

}
