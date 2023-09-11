using System;
using System.Collections.Generic;
using Sasaki.Common;
using ViewObjects.Common;

namespace ViewObjects.References
{

  [Serializable]
  public class ViewObjectReference : IVersionReference, IViewObject
  {
    /// <summary>
    /// </summary>
    public ViewObjectReference()
    { }

    /// <summary>
    /// </summary>
    public ViewObjectReference(List<string> references)
    {
      this.references = references;
    }

    /// <summary>
    /// </summary>
    /// <param name="references"></param>
    /// <param name="viewId"></param>
    /// <param name="viewName"></param>
    public ViewObjectReference(List<string> references, string viewId, string viewName = null)
    {
      name = viewName;
      this.references = references;
      appId = ObjUtils.CheckIfValidId(viewId);
    }

    /// <summary>
    /// </summary>
    /// <param name="references"></param>
    /// <param name="type"></param>
    /// <param name="viewId"></param>
    /// <param name="viewName"></param>
    public ViewObjectReference(List<string> references, Type type, string viewId, string viewName = null)
    {
      this.type = type;
      name = viewName;
      this.references = references;
      appId = ObjUtils.CheckIfValidId(viewId);
    }

    /// <inheritdoc />
    public string appId { get; protected set; }

    /// <inheritdoc />
    public string name { get; set; }

    /// <inheritdoc />
    public Type type { get; protected set; }

    /// <inheritdoc />
    public List<string> references { get; protected set; } = new();
  }

}
