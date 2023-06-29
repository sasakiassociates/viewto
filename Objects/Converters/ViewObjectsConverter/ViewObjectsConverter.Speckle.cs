using Speckle.Core.Models;
using System.Collections.Generic;
using System.Linq;
using ViewObjects.Clouds;
using ViewObjects.Common;
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

    private VS.ViewStudy StudyToSpeckle(ISasakiStudy<IViewObject> obj)
    {

      var result = new VS.ViewStudy() {ViewId = obj.ViewId, ViewName = obj.ViewName, objects = new List<VS.ViewObjectBase>()};

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

    private VS.ContentReference ViewContentToSpeckle(ContentReference obj)
    {
      return new VS.ContentReference(obj);
    }

    private VS.ViewCloudReference ViewCloudToSpeckle(ViewCloudReference obj)
    {
      return new VS.ViewCloudReference(obj);
    }

    private VS.ViewObjectReference ReferenceToSpeckle(IReferenceObject obj)
    {
      return new VS.ViewObjectReference(obj.References, obj.ViewId, obj.ViewId) { };
    }

    private VS.Layout LayoutToSpeckle(ILayout obj)
    {
      return new VS.Layout(obj.Viewers);
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
      return new VS.ResultCloud(obj.Points, obj.Data.Where(x => x != null).Select(ResultCloudDataToSpeckle).ToList(), obj.ViewId);
    }

    private VS.ResultCloudData ResultCloudDataToSpeckle(IResultCloudData obj)
    {
      return new VS.ResultCloudData(obj.values, obj.info, obj.count);
    }

    private IViewObject HandleDefault(Base @base)
    {
      var obj = @base.SearchForType<VS.ViewObjectBase>(true);

      return obj == null ? null : ConvertToNativeViewObject(obj);

    }

  }

}
