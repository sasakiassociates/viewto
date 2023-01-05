using Speckle.Core.Kits;
using Speckle.Core.Models;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using ViewObjects.Contents;

namespace ViewObjects.Speckle
{

  /// <summary>
  ///   View Content Object adapted to <see cref="Base" />
  /// </summary>
  public class ContentReference : ViewObjectReference<References.ContentReference>, IContent
  {

    const string CONTENT_TYPE = "Content_Type";

    /// <summary>
    /// 
    /// </summary>
    public ContentReference()
    { }

    /// <inheritdoc />
    [SchemaInfo("View Content", "Simple Object type for structuring geometry for a view study", ViewObject.Schema.Category, "Objects")]
    public ContentReference(References.ContentReference obj) : base(obj)
    {
      Type = obj.Type;
      References = obj.References;
      ContentType = obj.ContentType;
    }

    /// <inheritdoc />
    public ContentReference(ContentType type, List<string> references, string viewId, string viewName = null) : base(references, viewId, viewName)
    {
      ContentType = type;
    }

    /// <inheritdoc />
    [JsonIgnore] public ContentType ContentType
    {
      get => (ContentType)Enum.Parse(typeof(ContentType), (string)this[CONTENT_TYPE]);
      set => this[CONTENT_TYPE] = value.ToString();
    }

    /// <inheritdoc />
    public ViewColor Color { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool Equals(IContent obj)
    {
      return obj != default(object) && ViewId.Equals(obj.ViewId) && ContentType == obj.ContentType;
    }

  }

}
