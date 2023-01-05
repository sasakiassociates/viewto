using System;
using System.Collections.Generic;
using ViewObjects.Common;

namespace ViewObjects.Clouds;

public class ResultCloud : IResultCloud, IViewObject
{

  public ResultCloud()
  {
    ViewId = ObjUtils.InitGuid;
  }

  public ResultCloud(CloudPoint[] points, List<IResultCloudData> data, string viewId = null)
  {
    Points = points;
    Data = data;
    ViewId = ObjUtils.CheckIfValidId(viewId);
  }

  /// <inheritdoc />
  public string ViewId { get; }

  /// <inheritdoc />
  public CloudPoint[] Points { get; set; } = Array.Empty<CloudPoint>();

  /// <inheritdoc />
  public List<IResultCloudData> Data { get; set; } = new();
}
