using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewObjects.Cloud;
using ViewTo.Primers;

namespace ViewTo
{

  public static partial class Commander
  {

    /// <summary>
    ///   Checks for any linked clouds connected to this target
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static List<CloudShell> SearchForClouds(this ITargetContent obj)
    {
      var items = new List<CloudShell>();
      if (obj.bundles.Valid())
        foreach (var p in obj.bundles)
          if (p is IViewerBundleLinked pl)
            items.AddRange(pl.linkedClouds);

      return items;
    }

    internal static List<ComparedCloud> GetClouds(this ITargetContent obj)
    {
      var items = new List<ComparedCloud>();
      if (obj.bundles.Valid())
        foreach (var p in obj.bundles)
          if (p is IViewerBundleLinked pl)
            items.AddRange(pl.linkedClouds.Select(cl => new ComparedCloud(cl.objId, obj.viewName)));

      return items;
    }
  }
}
