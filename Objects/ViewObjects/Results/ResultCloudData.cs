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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="values"></param>
    /// <param name="content"></param>
    /// <param name="count"></param>
    public ResultCloudData(List<int> values, IContentOption content, int count)
    {
      this.info = content;
      this.values = values;
      this.count = count;
    }

    public ResultCloudData(List<int> values, IContentInfo target, IContentInfo content, ViewContentType type, int count)
    {
      this.info = new ContentOption(target, content, type);
      this.values = values;
      this.count = count;
    }

    /// <inheritdoc />
    public IContentOption info { get; }
    /// <inheritdoc />
    public int count { get; }
    /// <inheritdoc />
    public List<int> values { get; }
  }


}
