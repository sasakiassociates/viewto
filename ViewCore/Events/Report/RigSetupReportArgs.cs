using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewObjects.Cloud;
using ViewTo.Events.Args;

namespace ViewTo.Events.Report
{
  public class RigSetupReportArgs : AReportEventArgs
  {

    public readonly List<IViewerBundle> bundles;
    public readonly List<CloudShell> clouds;
    public readonly List<ViewColor> colors;

    public RigSetupReportArgs(List<IViewerBundle> bundles, List<ViewColor> colors, List<CloudShell> clouds)
    {
      this.bundles = bundles.Valid() ? bundles : new List<IViewerBundle>();
      this.colors = colors.Valid() ? colors : new List<ViewColor>();
      this.clouds = clouds.Valid() ? clouds : new List<CloudShell>();

      message = "Rig Primed!\n" + $"Bundles:{bundles.Count}\nViewers:{bundles.Sum(b => b?.layouts?.Count ?? 0)}\nPointCount:{clouds.Sum(item => item.count)} ";
    }
  }
}
