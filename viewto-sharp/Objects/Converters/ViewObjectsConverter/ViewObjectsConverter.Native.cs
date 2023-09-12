using System.Collections.Generic;
using System.Linq;
using ViewObjects.Clouds;
using Sasaki.Common;
using ViewObjects.References;
using ViewObjects.Results;
using ViewObjects.Studies;
using ViewObjects.Systems;
using ViewObjects.Systems.Layouts;

namespace ViewObjects.Converter
{

  public partial class ViewObjectsConverter
  {
    IViewObject StudyToNative(Speckle.ViewStudy obj)
    {
      var result = new ViewStudy() {appId = obj.guid, name = obj.name, items = new List<IViewObject>()};

      foreach(var o in obj.objects)
      {
        var res = ConvertToNativeViewObject(o);
        if(res == null)
        {
          // throw 
          continue;
        }
        result.items.Add(res);
      }

      return result;
    }

    IViewObject LayoutToNative(ILayout obj)
    {
      return new Layout(obj.viewers);
    }

    IViewObject ViewerToNative(IViewer<Speckle.Layout> o)
    {
      return new Layouts(o.Layouts.Where(x => x != null).Select(LayoutToNative).Cast<ILayout>().ToList());
    }

    IViewObject ViewerToNative(IViewerLinked<Speckle.Layout> o)
    {
      return new ViewerLinked(o.Layouts.Where(x => x != null).Select(LayoutToNative).Cast<ILayout>().ToList(), o.Clouds);
    }

    IViewObject ResultCloudToNative(Speckle.ResultCloud obj)
    {
      return new ResultCloud(obj.Points, obj.layers.Where(x => x != null).Select(ResultCloudDataToNative).ToList(), obj.guid);
    }

    IResultLayer ResultCloudDataToNative(IResultLayer obj)
    {
      return new ViewResultLayer(obj.values, obj.info);
    }

    IViewObject ContentReferenceToNative(Speckle.ContextReference obj)
    {
      return new ContextReferences(obj.references, obj.contextType, obj.appId, obj.name);
    }

    IViewObject ViewCloudReferenceToNative(IHaveRefs obj)
    {
      return new ViewCloudReference(obj.items, obj.appId);
    }

    ViewObjectReference ReferenceToNative(IHaveRefs obj)
    {
      return new ViewObjectReference(obj.items, obj.type, obj.appId, obj.name);
    }
  }

}
