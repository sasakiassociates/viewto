using System;
using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewObjects.Clouds;
using ViewObjects.Common;
using ViewObjects.Contents;

namespace ViewTo
{

  [Serializable]
  public class ExplorerMetaData
  {
    public ExplorerMetaData()
    { }

    public ExplorerMetaData(IResultCloud cloud)
    {
      options = cloud.GetAllOpts();

      if(options.Valid())
      {
        activeTarget = options.FirstOrDefault().target;
        activeStage = options.FirstOrDefault().stage;
        activeOptions = new List<ContentOption>() {options.FirstOrDefault()};
      }
    }

    /// <summary>
    /// the content info for the active target 
    /// </summary>
    public IContentInfo activeTarget { get; internal set; }

    /// <summary>
    /// the current stage that explorer is using to visualize the data
    /// </summary>
    public ViewContentType activeStage { get; internal set; }

    /// <summary>
    /// A cached list of the active options used in visualization of the explorer
    /// </summary>
    public List<ContentOption> activeOptions { get; internal set; } = new List<ContentOption>();

    /// <summary>
    /// The cached list of all options from the result data 
    /// </summary>
    public List<ContentOption> options { get; internal set; } = new List<ContentOption>();

  }

}
