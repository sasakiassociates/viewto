using System;
using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewObjects.Cloud;
using ViewTo.Events.Report;

namespace ViewTo
{
  public static partial class Commander
  {
    public static bool CanRun(this IRig obj)
    {
      return obj.clouds != null && obj.clouds.Any() && obj.globalParams != null && obj.globalParams.Any() && obj.globalColors != null && obj.globalColors.Any();
    }

    public static void Load(this IRig obj, string cloudID, CloudPoint[] points)
    {
      Console.WriteLine(points.Length);
      obj.clouds ??= new Dictionary<string, CloudPoint[]>();
      obj.clouds.Add(cloudID, points);
    }

    public static void Load(this IRig obj, List<ViewColor> globalColors, IEnumerable<IRigParam> bundles)
    {
      obj.globalColors = globalColors;
      obj.globalParams = bundles.ToList();
    }

    internal static void ReportSetup(this IRig obj, Action<RigSetupReportArgs> args)
    {
      var bundles = new List<IViewerBundle>();

      foreach (var g in obj.globalParams)
        bundles.AddRange(g.bundles);

      var clouds = obj.clouds.Select(p => new CloudShell(typeof(ViewCloud), p.Key, p.Value?.Length ?? 0))
        .ToList();

      args?.Invoke(new RigSetupReportArgs(bundles, obj.globalColors, clouds));
    }
  }
}
