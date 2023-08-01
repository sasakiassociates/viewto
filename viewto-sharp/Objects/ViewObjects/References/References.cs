using System.Collections.Generic;
using ViewObjects.Common;

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
      Type = typeof(TObj);
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

      Type = obj.GetType();

      if(typeof(IId).IsAssignableFrom(Type) && obj is IId i)
      {
        ViewId = i.ViewId;
      }

      if(typeof(INameable).IsAssignableFrom(Type) && obj is INameable n)
      {
        ViewName = n.ViewName;
      }
    }

  }

}
