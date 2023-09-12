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
  public class ContextReference : ViewObjectReference<References.ContextReferences>, IContext
  {

    private const string CONTENT_TYPE = "Content_Type";

    /// <summary>
    /// 
    /// </summary>
    public ContextReference()
    { }

    /// <inheritdoc />
    [SchemaInfo("View Content", "Simple Object type for structuring geometry for a view study", ViewObject.Schema.Category, "Objects")]
    public ContextReference(References.ContextReferences obj) : base(obj)
    {
      this.type = obj.type;
      this.contextType = obj.contentType;
      this.appId = obj.appId;
      this.name = obj.name;
      this.references = obj.items;
    }

    /// <inheritdoc />
    public ContextReference(ViewContextType contextType, List<string> references, string viewId, string viewName = null) : base(references, viewId, viewName)
    {
      this.contextType = contextType;
      this.appId = viewId;
      this.name = viewName;
      this.references = references;
    }

    /// <inheritdoc />
    [JsonIgnore] public ViewContextType contextType
    {
      get => (ViewContextType)Enum.Parse(typeof(ViewContextType), (string)this[CONTENT_TYPE]);
      set => this[CONTENT_TYPE] = value.ToString();
    }

    /// <inheritdoc />
    public ViewColor Color { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool Equals(IContext obj)
    {
      return obj != default(object) && appId.Equals(obj.appId) && contextType == obj.contentType;
    }

  }

}
