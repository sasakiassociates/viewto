using System.Collections.Generic;
using ViewObjects;

namespace ViewTo
{
  public interface IExplorerData
  {
    public ResultType activeType { get; set; }

    public List<IResultData> storedData { get; }

    public List<string> targets { get; }
  }
}
