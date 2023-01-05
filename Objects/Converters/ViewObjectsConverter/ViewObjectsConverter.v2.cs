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
    IViewObject StudyToNative(VS.ViewStudy obj)
    {
      return new ViewStudy
      (
        obj.Objects.Valid() ? obj.Objects.Where(x => x != null).Select(ConvertToNativeViewObject).ToList()
          : new List<IViewObject>(),
        obj.ViewName,
        obj.ViewId
      );
    }

    IViewObject LayoutToNative(ILayout obj)
    {
      return new Layout(obj.Viewers);
    }

    IViewObject ViewerToNative(IViewer<VS.Layout> o)
    {
      return new Viewer(o.Layouts.Where(x => x != null).Select(LayoutToNative).Cast<ILayout>().ToList());
    }

    IViewObject ViewerToNative(IViewerLinked<VS.Layout> o)
    {
      return new ViewerLinked(o.Layouts.Where(x => x != null).Select(LayoutToNative).Cast<ILayout>().ToList(), o.Clouds);
    }

    IViewObject ResultCloudToNative(VS.ResultCloud obj)
    {
      return new ResultCloud(obj.Points, obj.Data.Where(x => x != null).Select(ResultCloudDataToNative).ToList(), obj.ViewId);
    }

    IResultCloudData ResultCloudDataToNative(IResultCloudData obj)
    {
      return new ResultCloudData(obj.Values, obj.Option, obj.Layout);
    }

    IViewObject ContentReferenceToNative(VS.ContentReference obj)
    {
      return new ContentReference(obj.References, obj.ContentType, obj.ViewId, obj.ViewName);
    }

    IViewObject ViewCloudReferenceToNative(IReferenceObject obj)
    {
      return new ViewCloudReference(obj.References, obj.ViewId);
    }

    ViewObjectReference ReferenceToNative(IReferenceObject obj)
    {
      return new ViewObjectReference(obj.References, obj.Type, obj.ViewId, obj.ViewName);
    }

    VS.ViewStudy StudyToSpeckle(IViewStudy<IViewObject> obj)
    {
      return new VS.ViewStudy
      {
        Objects = obj.Objects.Valid() ? obj.Objects.Where(x => x != null).Select(ConvertToSpeckleViewObject).ToList() : new List<VS.ViewObjectBase>(),
        ViewName = obj.ViewName,
        ViewId = obj.ViewId
      };
    }

    // VS.ContentReference ViewContentToSpeckle(Content obj) => new VS.ContentReference(obj, new List<string>());

    // VS.ContentReference ViewContentToSpeckle(IContent obj) => new VS.ContentReference(obj, new List<string>());

    VS.ContentReference ViewContentToSpeckle(ContentReference obj)
    {
      return new VS.ContentReference(obj);
    }

    VS.ViewCloudReference ViewCloudToSpeckle(ViewCloudReference obj)
    {
      return new VS.ViewCloudReference(obj);
    }

    VS.ViewObjectReference ReferenceToSpeckle(IReferenceObject obj)
    {
      return new VS.ViewObjectReference(obj.References, obj.ViewId, obj.ViewId) { };
    }

    VS.Layout LayoutToSpeckle(ILayout obj)
    {
      return new VS.Layout(obj.Viewers);
    }

    VS.Viewer ViewerToSpeckle(IViewer<ILayout> o)
    {
      return new VS.Viewer(
        o.Layouts.Where(x => x != null).Select(LayoutToSpeckle).ToList());
    }

    VS.ViewerLinked ViewerToSpeckle(IViewerLinked o)
    {
      return new VS.ViewerLinked(
        o.Layouts.Where(x => x != null).Select(LayoutToSpeckle).ToList(), o.Clouds);
    }

    VS.ResultCloud ResultCloudToSpeckle(IResultCloud obj)
    {
      return new VS.ResultCloud(obj.Points, obj.Data.Where(x => x != null).Select(ResultCloudDataToSpeckle).ToList(), obj.ViewId);
    }

    VS.ResultCloudData ResultCloudDataToSpeckle(IResultCloudData obj)
    {
      return new VS.ResultCloudData(obj.Values, obj.Option, obj.Layout);
    }

    //TODO: Support getting list of objects from search

    IViewObject HandleDefault(Base @base)
    {
      var obj = @base.SearchForType<VS.ViewObjectBase>(true);

      return obj == null ? null : ConvertToNativeViewObject(obj);

    }

  }

}
