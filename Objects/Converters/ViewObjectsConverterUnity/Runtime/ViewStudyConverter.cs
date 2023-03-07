using Speckle.ConnectorUnity.Converter;
using Speckle.Core.Models;
using System.Collections.Generic;
using UnityEngine;
using VS = ViewObjects.Speckle;
using VU = ViewObjects.Unity;

namespace ViewObjects.Converter.Unity
{

  [CreateAssetMenu(menuName = ViewObjectUnity.ASSET_MENU + "Create " + nameof(ViewStudyConverter), fileName = nameof(ViewStudyConverter), order = 0)]
  public class ViewStudyConverter : ComponentConverter<VS.ViewStudy, VU.ViewStudy>
  {
    public override Base ConvertComponent(VU.ViewStudy component)
    {
      return null;
    }

    protected override void ConvertBase(VS.ViewStudy obj, ref VU.ViewStudy instance)
    {
      instance.ViewId = obj.ViewId;
      instance.ViewName = obj.ViewName;

      // necessary since we cant just add a view object to this list like that
      var objs = new List<IViewObject>();

      foreach(var o in obj.objects)
      {
        VU.ViewObjectMono mono = null;
        var item = parent.ConvertToNative(o);

        if(item is GameObject go)
        {
          mono = go.GetComponent<VU.ViewObjectMono>();
        }
        if(item is VU.ViewObjectMono vo)
        {
          mono = vo;
        }

        if(mono == null)
        {
          Debug.Log($"{o.speckle_type} was not converted correctly");
          continue;
        }

        objs.Add(mono);
        mono.transform.SetParent(instance.transform);

      }
      instance.objects = objs;


    }
  }

}
