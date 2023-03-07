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

    private const string CONTENT_TYPE = "Content_Type";

    /// <summary>
    /// 
    /// </summary>
    public ContentReference()
    { }

    /// <inheritdoc />
    [SchemaInfo("View Content", "Simple Object type for structuring geometry for a view study", ViewObject.Schema.Category, "Objects")]
    public ContentReference(References.ContentReference obj) : base(obj)
    {
      this.Type = obj.Type;
      this.type = obj.type;
      this.ViewId = obj.ViewId;
      this.ViewName = obj.ViewName;
      this.References = obj.References;
    }

    /// <inheritdoc />
    public ContentReference(ViewContentType type, List<string> references, string viewId, string viewName = null) : base(references, viewId, viewName)
    {
      this.type = type;
      this.ViewId = viewId;
      this.ViewName = viewName;
      this.References = references;
    }

    /// <inheritdoc />
    [JsonIgnore] public ViewContentType type
    {
      get => (ViewContentType)Enum.Parse(typeof(ViewContentType), (string)this[CONTENT_TYPE]);
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
      return obj != default(object) && ViewId.Equals(obj.ViewId) && type == obj.type;
    }

  }

}
