using System.Collections.Generic;

namespace ViewObjects.Clouds
{

  public interface IResultCloud : IResultCloud<IResultCloudData>
  { }

  public interface IResultCloud<TData> : IViewCloud where TData : IResultCloudData
  {
    /// <summary>
    ///   The view analysis data gathered
    /// </summary>
    public List<TData> Data { get; set; }
  }

}
