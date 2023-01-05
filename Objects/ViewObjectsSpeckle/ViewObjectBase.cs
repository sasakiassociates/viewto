using Speckle.Core.Models;

namespace ViewObjects.Speckle
{

  /// <summary>
  /// </summary>
  public abstract class ViewObjectBase : Base, IViewObject
  {

    /// <summary>
    /// </summary>
    public ViewObjectBase()
    { }

    /// <summary>
    ///   Returns <see cref="ViewObjectBase" />
    /// </summary>
    public override string speckle_type => GetType().ToString();
  }

}
