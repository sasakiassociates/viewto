using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewTo.Events.Args;

namespace ViewTo.Events.Prime
{
  public class PrimeViewerBundleArgs : PrimeProcessArgs
  {
    public readonly IEnumerable<IViewerBundle> bundles;

    public PrimeViewerBundleArgs(IEnumerable<IViewerBundle> bundles)
    {
      this.bundles = bundles;
    }

    public override bool success => bundles.Any();
  }
}
