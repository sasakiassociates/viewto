using System.Linq;
using ViewObjects;
using ViewTo.Events.Args;

namespace ViewTo.Events.Prime
{
  /// <summary>
  ///   Simple sealed class that stores primed cloud data points for instancing
  /// </summary>
  public sealed class PrimeCloudArgs : PrimeProcessArgs
  {
    public readonly string id;
    public readonly CloudPoint[] points;
    public PrimeCloudArgs(string id, CloudPoint[] points)
    {
      this.id = id;
      this.points = points;
      message = "Primed Cloud-" + (success ? $"{id} with {points.Length}" : "Failed ");
    }
    public override bool success => id.Valid() && points != null && points.Any();
  }

}
