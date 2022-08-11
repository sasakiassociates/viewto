using System.Collections.Generic;
using ViewObjects;

namespace ViewTo.Events.Prime
{
  public sealed class PrimeViewerBundleIsolatedArgs : PrimeViewerBundleArgs
  {
    public readonly ViewColor viewColor;
    public PrimeViewerBundleIsolatedArgs(List<IViewerBundle> bundles, ViewColor color) : base(bundles)
    {
      viewColor = color;
    }
  }
}
