using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Sasaki.Common;

namespace ViewObjects.Speckle
{

  /// <summary>
  /// </summary>
  public class ViewObjectReference : ViewObjectBase, IVersionReference
  {
    /// <summary>
    /// 
    /// </summary>
    public ViewObjectReference()
    { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="references"></param>
    public ViewObjectReference(List<string> references)
    {
      this.references = references;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="references"></param>
    /// <param name="viewId"></param>
    /// <param name="viewName"></param>
    public ViewObjectReference(List<string> references, string viewId, string viewName = null)
    {
      this.references = references;
      appId = SasakiTools.CheckIfValidId(viewId);
      name = viewName;
    }

    /// <inheritdoc />
    public List<string> references { get; set; }

    /// <inheritdoc />
    public string name { set; get; }

    /// <inheritdoc />
    public string appId { get; set; }

    /// <inheritdoc />
    [JsonIgnore] public Type type { get; set; }
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
    { }

    /// <summary>
    /// </summary>
    /// <param name="obj"></param>
    public ViewObjectReference(TObj obj)
    {
      type = typeof(TObj);

      if(typeof(IHaveId).IsAssignableFrom(type) && obj is IHaveId i)
      {
        appId = SasakiTools.CheckIfValidId(i.appId);
      }

      if(typeof(IHaveName).IsAssignableFrom(type) && obj is IHaveName n)
      {
        name = n.name;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="references"></param>
    public ViewObjectReference(TObj obj, List<string> references) : base(references)
    {
      type = typeof(TObj);

      if(typeof(IHaveId).IsAssignableFrom(type) && obj is IHaveId i)
      {
        appId = SasakiTools.CheckIfValidId(i.appId);
      }

      if(typeof(IHaveName).IsAssignableFrom(type) && obj is IHaveName n)
      {
        name = n.name;
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
      type = typeof(TObj);
    }

  }

}
