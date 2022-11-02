using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace ViewObjects.Speckle
{

  /// <summary>
  /// </summary>
  public class ViewObjectReference : ViewObjectBase, IReferenceObject
  {
    /// <summary>
    /// 
    /// </summary>
    public ViewObjectReference()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="references"></param>
    public ViewObjectReference(List<string> references)
    {
      References = references;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="references"></param>
    /// <param name="viewId"></param>
    /// <param name="viewName"></param>
    public ViewObjectReference(List<string> references, string viewId, string viewName = null)
    {
      References = references;
      ViewId = ObjUtils.CheckIfValidId(viewId);
      ViewName = viewName;
    }

    /// <inheritdoc />
    public List<string> References { get; set; }

    /// <inheritdoc />
    public string ViewName { set; get; }

    /// <inheritdoc />
    public string ViewId { get; set; }

    /// <inheritdoc />
    [JsonIgnore] public Type Type { get; set; }
  }

  /// <summary>
  /// </summary>
  /// <typeparam name="TObj"></typeparam>
  public class ViewObjectReference<TObj> : ViewObjectReference where TObj : IViewObject
  {

    /// <summary>
    /// 
    /// </summary>
    public ViewObjectReference()
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="obj"></param>
    public ViewObjectReference(TObj obj)
    {
      Type = typeof(TObj);

      if (typeof(IId).IsAssignableFrom(Type) && obj is IId i)
      {
        ViewId = ObjUtils.CheckIfValidId(i.ViewId);
      }

      if (typeof(INameable).IsAssignableFrom(Type) && obj is INameable n)
      {
        ViewName = n.ViewName;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="references"></param>
    public ViewObjectReference(TObj obj, List<string> references) : base(references)
    {
      Type = typeof(TObj);

      if (typeof(IId).IsAssignableFrom(Type) && obj is IId i)
      {
        ViewId = ObjUtils.CheckIfValidId(i.ViewId);
      }

      if (typeof(INameable).IsAssignableFrom(Type) && obj is INameable n)
      {
        ViewName = n.ViewName;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="references"></param>
    /// <param name="viewId"></param>
    /// <param name="viewName"></param>
    public ViewObjectReference(List<string> references, string viewId, string viewName = null) : base(references, viewId, viewName)
    {
      Type = typeof(TObj);
    }

  }
}
