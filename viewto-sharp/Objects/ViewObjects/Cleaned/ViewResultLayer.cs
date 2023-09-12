using Sasaki.Analysis;

namespace Sasaki.ViewObjects;


public class ViewResultLayer : ResultLayer
{
  /// <summary>
  /// An additional property for View To to know some data behind what conditions were active with these results
  /// </summary>
  public ResultCondition info {get;set;}
}