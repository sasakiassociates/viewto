using System.Collections.Generic;
using System.Linq;
using ViewObjects.Clouds;
using ViewObjects.Common;
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
      var result = new ViewStudy() {ViewId = obj.ViewId, ViewName = obj.ViewName, objects = new List<IViewObject>()};

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
      return new ResultCloud(obj.Points, obj.Data.Where(x => x != null).Select(ResultCloudDataToNative).ToList(), obj.ViewId);
    }

    IResultCloudData ResultCloudDataToNative(IResultCloudData obj)
    {
      return new ResultCloudData(obj.values, obj.info);
    }

    IViewObject ContentReferenceToNative(Speckle.ContentReference obj)
    {
      return new ContentReference(obj.References, obj.type, obj.ViewId, obj.ViewName);
    }

    IViewObject ViewCloudReferenceToNative(IReferenceObject obj)
    {
      return new ViewCloudReference(obj.References, obj.ViewId);
    }

    ViewObjectReference ReferenceToNative(IReferenceObject obj)
    {
      return new ViewObjectReference(obj.References, obj.Type, obj.ViewId, obj.ViewName);
    }
  }

}
