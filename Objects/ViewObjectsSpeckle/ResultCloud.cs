using Speckle.Core.Kits;
using Speckle.Core.Logging;
using Speckle.Core.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace ViewObjects.Speckle
{
  /// <summary>
  /// </summary>
  public class ResultCloud : ViewObjectBase, IResultCloud<ResultCloudData>
  {

    /// <summary>
    /// </summary>
    public ResultCloud()
    {
    }

    /// <summary>
    ///   Constructs a Result cloud
    /// </summary>
    /// <param name="points"></param>
    /// <param name="data"></param>
    /// <param name="viewId"></param>
    [SchemaInfo("Result Cloud", "A view analysis cloud with result data attached", ViewObject.Schema.Category, "Objects")]
    public ResultCloud(CloudPoint[] points, List<ResultCloudData> data, string viewId = null)
    {
      Data = data;
      Points = points;
      ViewId = ObjUtils.CheckIfValidId(viewId);
    }

    /// <summary>
    ///   List of point positions as x,y,z
    ///   This should be set using <see cref="Points" />
    /// </summary>
    [DetachProperty][Chunkable(31250)] public List<double> Positions { get; set; } = new List<double>();

    /// <summary>
    ///   List of meta data for <see cref="CloudPoint" />
    ///   This should be set using <see cref="Points" />
    /// </summary>
    [DetachProperty][Chunkable(31250)] public List<string> MetaData { get; set; } = new List<string>();

    /// <inheritdoc />
    public string ViewId { get; set; }

    /// <inheritdoc />
    public List<ResultCloudData> Data { get; set; } = new List<ResultCloudData>();

    /// <inheritdoc />
    [JsonIgnore] public CloudPoint[] Points
    {
      get
      {
        if (Positions.Count % 3 != 0)
        {
          throw new SpeckleException($"{nameof(ResultCloud)}.{nameof(Positions)} list is malformed: expected length to be multiple of 3");
        }

        var points = new CloudPoint[Positions.Count / 3];

        for (int i = 2, c = 0; i < Positions.Count; i += 3, c++)
        {
          points[c] = new CloudPoint(Positions[i - 2], Positions[i - 1], Positions[i], 0, 0, 0, MetaData[c] ?? "empty");
        }

        return points;
      }
      set
      {
        Positions = new List<double>();
        MetaData = new List<string>();

        foreach (var point in value)
        {
          Positions.Add(point.x);
          Positions.Add(point.y);
          Positions.Add(point.z);
          MetaData.Add(point.meta);
        }
      }
    }
  }
}
