using Sasaki.Common;

namespace Sasaki.ViewObjects;


public interface IViewObject : IObject
{ }

public class ViewObject : IViewObject, IHaveName
{
  public string appId {get;set;}
  public string name {get;set;}
}
