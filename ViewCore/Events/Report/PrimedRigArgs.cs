using System.Collections.Generic;
using ViewObjects;
using ViewTo.Events.Args;

namespace ViewTo.Events.Report
{
  public class PrimedRigArgs : AReportEventArgs
  {
    public readonly Dictionary<string, CloudPoint[]> clouds;
    public readonly List<ViewColor> globalColors;
    public readonly List<IRigParam> globalParams;

    public PrimedRigArgs(List<IRigParam> globalParams, List<ViewColor> globalColors, Dictionary<string, CloudPoint[]> clouds)
    {
      this.globalParams = globalParams;
      this.globalColors = globalColors;
      this.clouds = clouds;
    }

    public PrimedRigArgs(IRig rig)
    {
      if (rig != null)
      {
        clouds = rig.clouds;
        globalParams = rig.globalParams;
        globalColors = rig.globalColors;
      }
    }
  }
}
