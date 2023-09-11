using Sasaki.Common;

namespace ViewObjects.References
{

  public interface IViewObjectReference : IViewObject
  {
    public IVersionReference Reference { get; set; }
  }

}
