using Objects.Geometry;
using Speckle.ConnectorUnity.Ops;
using Speckle.Core.Models;
using System.Linq;
using UnityEngine;
using ViewObjects.Clouds;
using VS = ViewObjects.Speckle;
using VU = ViewObjects.Unity;
using ConnectorUtils = Speckle.ConnectorUnity.Utils;

namespace ViewObjects.Converter.Unity
{

  public static class ViewObjectUnity
  {
    public const string ASSET_MENU = "ViewTo/";

    public static bool TryLoadReference<TBase, TMono>(this TMono mono, Base refObj, out TBase obj) where TBase : Base where TMono : VU.ViewObjectMono
    {
      obj = default(TBase);

      if(mono == null || refObj == null)
      {
        Debug.Log("Objects are not valid\n" +
                  $"{typeof(TMono)}={(mono == null ? "null" : "valid")}\n" +
                  $"{typeof(TBase)}={(refObj == null ? "null" : "valid")}"
        );

        return false;
      }

      obj = refObj.SearchForTypeSync<TBase>(true);

      Debug.Log($"{typeof(TBase)} was {(obj == null ? "not found" : "found")}");

      return obj != null;
    }

    public static void LoadReference(this VU.ViewCloud cloud, Base refObj)
    {
      cloud.TryLoadReference(refObj, out Pointcloud co);

      cloud.Points =
        ConnectorUtils.ToVectorArray(co.points, co.units).Select(v => new CloudPoint(v.x, v.y, v.z)).ToArray();
    }
  }

}
