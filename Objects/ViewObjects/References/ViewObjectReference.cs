using System;
using System.Collections.Generic;
using ViewObjects.Common;

namespace ViewObjects.References;

[Serializable]
public class ViewObjectReference : IReferenceObject, IViewObject
{
  /// <summary>
  /// </summary>
  public ViewObjectReference()
  { }

  /// <summary>
  /// </summary>
  public ViewObjectReference(List<string> references)
  {
    References = references;
  }

  /// <summary>
  /// </summary>
  /// <param name="references"></param>
  /// <param name="viewId"></param>
  /// <param name="viewName"></param>
  public ViewObjectReference(List<string> references, string viewId, string viewName = null)
  {
    ViewName = viewName;
    References = references;
    ViewId = ObjUtils.CheckIfValidId(viewId);
  }

  /// <summary>
  /// </summary>
  /// <param name="references"></param>
  /// <param name="type"></param>
  /// <param name="viewId"></param>
  /// <param name="viewName"></param>
  public ViewObjectReference(List<string> references, Type type, string viewId, string viewName = null)
  {
    Type = type;
    ViewName = viewName;
    References = references;
    ViewId = ObjUtils.CheckIfValidId(viewId);
  }

  /// <inheritdoc />
  public string ViewId { get; protected set; }

  /// <inheritdoc />
  public string ViewName { get; set; }

  /// <inheritdoc />
  public Type Type { get; protected set; }

  /// <inheritdoc />
  public List<string> References { get; protected set; } = new();
}
