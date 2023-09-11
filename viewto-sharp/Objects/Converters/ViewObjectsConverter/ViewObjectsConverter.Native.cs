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
      var result = new ViewStudy() {appId = obj.guid, name = obj.name, objects = new List<IViewObject>()};

      foreach(var o in obj.objects)
      {
        var res = ConvertToNativeViewObject(o);
        if(res == null)
        {
          // throw 
          continue;
        }
        result.objects.Add(res);
      }

      return result;
    }

    IViewObject LayoutToNative(ILayout obj)
    {
      return new Layout(obj.Viewers);
    }

    IViewObject ViewerToNative(IViewer<Speckle.Layout> o)
    {
      return new Viewer(o.Layouts.Where(x => x != null).Select(LayoutToNative).Cast<ILayout>().ToList());
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
      return new ResultLayer(obj.values, obj.info);
    }

    IViewObject ContentReferenceToNative(Speckle.ContentReference obj)
    {
      return new ContentReference(obj.references, obj.contentType, obj.appId, obj.name);
    }

    IViewObject ViewCloudReferenceToNative(IVersionReference obj)
    {
      return new ViewCloudReference(obj.references, obj.appId);
    }

    ViewObjectReference ReferenceToNative(IVersionReference obj)
    {
      return new ViewObjectReference(obj.references, obj.type, obj.appId, obj.name);
    }
  }

}
