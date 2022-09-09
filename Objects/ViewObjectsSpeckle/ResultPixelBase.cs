using System.Collections.Generic;
using Speckle.Core.Models;

namespace ViewObjects.Speckle
{
  public class ResultPixelBase : ViewObjBase, IResultData
  {
    public ResultPixelBase()
    { }

    [DetachProperty] [Chunkable]
    public List<int> values { get; set; }

    public string content { get; set; }
    public string stage { get; set; }
    public string meta { get; set; }
    public int color { get; set; }
    public string layout { get; set; }
  }
}
