using System.Collections.Generic;
using Sasaki.Common;

namespace ViewObjects.References
{

  public abstract class ViewObjectReference<TObj> : ViewObjectReference where TObj : IViewObject
  {

    /// <summary>
    /// </summary>
    /// <param name="references"></param>
    /// <param name="viewId"></param>
    /// <param name="viewName"></param>
    protected ViewObjectReference(List<string> references, string viewId, string viewName = null) :
      base(references, viewId, viewName)
    {
      type = typeof(TObj);
    }

    /// <summary>
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="references"></param>
    protected ViewObjectReference(TObj obj, List<string> references) : base(references)
    {
      if(obj == null)
      {
        return;
      }

      type = obj.GetType();

      if(typeof(IHaveId).IsAssignableFrom(type) && obj is IHaveId i)
      {
        appId = i.appId;
      }

      if(typeof(IHaveName).IsAssignableFrom(type) && obj is IHaveName n)
      {
        name = n.name;
      }
    }

  }

}
