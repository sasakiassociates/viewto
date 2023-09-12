using System;
using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewObjects.Clouds;
using Sasaki.Common;
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
        activeTarget = options.FirstOrDefault().focus;
        activeStage = options.FirstOrDefault().stage;
        activeOptions = new List<ResultCondition>() {options.FirstOrDefault()};
      }
    }

    /// <summary>
    /// the content info for the active target 
    /// </summary>
    public IContextInfo activeTarget { get; internal set; }

    /// <summary>
    /// the current stage that explorer is using to visualize the data
    /// </summary>
    public ViewContextType activeStage { get; internal set; }

    /// <summary>
    /// A cached list of the active options used in visualization of the explorer
    /// </summary>
    public List<ResultCondition> activeOptions { get; internal set; } = new List<ResultCondition>();

    /// <summary>
    /// The cached list of all options from the result data 
    /// </summary>
    public List<ResultCondition> options { get; internal set; } = new List<ResultCondition>();

  }

}
