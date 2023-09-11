using System.Collections.Generic;
using ViewObjects.Clouds;
using ViewObjects.Results;

namespace ViewTo
{


  public interface IExplorer
  {
    /// <summary>
    ///   The heart and soul of the data being explored
    /// </summary>
    public IResultCloud cloud { get; }

    /// <summary>
    ///   Container for result values being explored
    /// </summary>
    public List<IResultLayer> data { get; }

    /// <summary>
    ///  A container of content options that relate to the result data
    /// </summary>
    public ExplorerMetaData meta { get; }

    /// <summary>
    ///  Set of data settings for the explorer to use
    /// </summary>
    public ExplorerSettings settings { get; set; }


    /// <summary>
    ///  Load in a new view study for the explorer to explore!
    /// </summary>
    /// <param name="obj">The view study to load in</param>
    public void Load(IResultCloud obj);
  }

}
