using System;
using System.Collections.Generic;
using ViewObjects.Clouds;
using ViewObjects.Contents;

namespace ViewObjects.Results
{

  /// <inheritdoc />
  [Serializable]
  public class ResultCloudData : IResultCloudData
  {

    public ResultCloudData()
    { }

    public ResultCloudData(List<int> values, IContentOption content, string layout)
    {
      this.info = content;
      this.values = values;
      this.layout = layout;
    }

    public ResultCloudData(List<int> values, IContentInfo target, IContentInfo content, ViewContentType type, string layout)
    {
      this.info = new ContentOption(target, content, type);
      this.values = values;
      this.layout = layout;
    }

    /// <inheritdoc />
    public IContentOption info { get; }
    /// <inheritdoc />
    public string layout { get; }
    /// <inheritdoc />
    public List<int> values { get; }
  }

}
