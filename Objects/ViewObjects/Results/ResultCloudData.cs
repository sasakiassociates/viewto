using System;
using System.Collections.Generic;
using ViewObjects.Clouds;
using ViewObjects.Contents;

namespace ViewObjects.Results;

[Serializable]
public class ResultCloudData : IResultCloudData
{

  /// <summary>
  /// </summary>
  public ResultCloudData()
  { }

  public ResultCloudData(List<int> values, IContentOption option, string layout)
  {
    Values = values;
    Option = option;
    Layout = layout;
  }

  /// <inheritdoc />
  public IContentOption Option { get; set; }

  /// <inheritdoc />
  public string Layout { get; set; }

  /// <inheritdoc />
  public List<int> Values { get; set; }
}
