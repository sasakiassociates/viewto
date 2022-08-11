using System.Collections.Generic;
using ViewObjects;
using ViewObjects.Cloud;
using ViewObjects.Viewer;

namespace ViewTo
{
  public static partial class Commander
  {

    internal static IEnumerable<CloudShell> GetClouds(this ViewerBundleLinked obj)
    {
      return obj != null && obj.linkedClouds.Valid() ? obj.linkedClouds : new List<CloudShell>();
    }

    internal static IEnumerable<CloudShell> GetClouds(this IEnumerable<ViewerBundleLinked> obj)
    {
      var shells = new List<CloudShell>();
      if (obj == null) return shells;

      foreach (var bundle in obj) shells.AddRange(bundle.linkedClouds);
      return shells;
    }
  }
}
