using Sasaki.Analysis;
using Sasaki.Common;
using System.Collections.Generic;

namespace Sasaki.ViewObjects;


public interface IViewResultLayer : IResultLayer
{
  /// <summary>
  /// descriptor for saying what context was used for this analysis
  /// </summary>
  public ResultCondition condition {get;}
}

public class ViewResultDataLayer : ViewObject, IViewResultLayer
{
  /// <inheritdoc />
  public int count => values.Valid() ? values.Count : 0;

  /// <inheritdoc />
  public List<int> values {get;} = new List<int>();

  /// <inheritdoc />
  public ResultCondition condition {get;set;}
}
