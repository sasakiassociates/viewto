using System;
using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewObjects.Clouds;
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
      activeTarget = options.FirstOrDefault().target;
      activeStage = options.FirstOrDefault().stage;
      activeOptions = new List<IContentInfo>() {options.FirstOrDefault().content};

    }

    /// <summary>
    /// the content info for the active target 
    /// </summary>
    public IContentInfo activeTarget;

    /// <summary>
    /// the current stage that explorer is using to visualize the data
    /// </summary>
    public ViewContentType activeStage;

    /// <summary>
    /// A cached list of the active options used in visualization of the explorer
    /// </summary>
    public List<IContentInfo> activeOptions = new List<IContentInfo>();

    /// <summary>
    /// The cached list of all options from the result data 
    /// </summary>
    public List<IContentOption> options = new List<IContentOption>();

  }

}
