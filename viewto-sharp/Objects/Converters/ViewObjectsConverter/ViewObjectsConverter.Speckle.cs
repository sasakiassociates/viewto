using Speckle.Core.Models;
using System.Collections.Generic;
using System.Linq;
using ViewObjects.Clouds;
using Sasaki.Common;
using ViewObjects.References;
using ViewObjects.Results;
using ViewObjects.Studies;
using ViewObjects.Systems;
using ViewObjects.Systems.Layouts;
using VS = ViewObjects.Speckle;
using VO = ViewObjects;

namespace ViewObjects.Converter
{

  /// <inheritdoc />
  public partial class ViewObjectsConverter
  {

    private VS.ViewStudy StudyToSpeckle(IStudy<IViewObject> obj)
    {

      var result = new VS.ViewStudy() {guid = obj.guid, name = obj.name, objects = new List<VS.ViewObjectBase>()};

      foreach(var o in obj.objects)
      {
        var res = ConvertToSpeckleViewObject(o);
        if(res == null)
        {
          // throw 
          continue;
        }
        result.objects.Add(res);
      }

      return result;
    }

    // VS.ContentReference ViewContentToSpeckle(Content obj) => new VS.ContentReference(obj, new List<string>());

    // VS.ContentReference ViewContentToSpeckle(IContent obj) => new VS.ContentReference(obj, new List<string>());

    private VS.ContextReference ViewContentToSpeckle(ContextReferences obj)
    {
      return new VS.ContextReference(obj);
    }

    private VS.ViewCloudReference ViewCloudToSpeckle(ViewCloudReference obj)
    {
      return new VS.ViewCloudReference(obj);
    }

    private VS.ViewObjectReference ReferenceToSpeckle(IHaveRefs obj)
    {
      return new VS.ViewObjectReference(obj.items, obj.appId, obj.appId) { };
    }

    private VS.Layout LayoutToSpeckle(ILayout obj)
    {
      return new VS.Layout(obj.viewers);
    }

    private VS.Viewer ViewerToSpeckle(IViewer<ILayout> o)
    {
      return new VS.Viewer(
        o.Layouts.Where(x => x != null).Select(LayoutToSpeckle).ToList());
    }

    private VS.ViewerLinked ViewerToSpeckle(IViewerLinked o)
    {
      return new VS.ViewerLinked(
        o.Layouts.Where(x => x != null).Select(LayoutToSpeckle).ToList(), o.Clouds);
    }

    private VS.ResultCloud ResultCloudToSpeckle(IResultCloud obj)
    {
      return new VS.ResultCloud(obj.Points, obj.layers.Where(x => x != null).Select(ResultCloudDataToSpeckle).ToList(), obj.guid);
    }

    private VS.ResultLayer ResultCloudDataToSpeckle(IResultLayer obj)
    {
      return new VS.ResultLayer(obj.values, obj.info, obj.count);
    }

    private IViewObject HandleDefault(Base @base)
    {
      var obj = @base.SearchForType<VS.ViewObjectBase>(true);

      return obj == null ? null : ConvertToNativeViewObject(obj);

    }

  }

}
