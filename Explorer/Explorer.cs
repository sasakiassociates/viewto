using System.Collections.Generic;
using ViewObjects.Clouds;
using ViewObjects.Results;

namespace ViewTo
{

  public class Explorer : IExplorer
  {

    /// <inheritdoc />
    public IResultCloud cloud { get; internal set; }

    /// <inheritdoc />
    public ExplorerSettings settings { get; set; } = new();

    /// <inheritdoc />
    public ExplorerMetaData meta { get; internal set; } = new();

    /// <inheritdoc />
    public List<IResultCloudData> data => cloud?.Data ?? new List<IResultCloudData>();

    /// <inheritdoc />
    public void Load(IResultCloud cloud)
    {
      if(cloud == null) return;

      this.cloud = cloud;
      this.meta = new ExplorerMetaData(cloud);

    }


  }

}
