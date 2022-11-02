using Speckle.Core.Models;
using System.Collections.Generic;
using System.Linq;
using VS = ViewObjects.Speckle;
using VO = ViewObjects;

namespace ViewObjects.Converter
{
  /// <inheritdoc />
  public partial class ViewObjectsConverter
  {
    private IViewObject StudyToNative(VS.ViewStudy obj)
    {
      return new ViewStudy
      (
        obj.Objects.Valid() ? obj.Objects.Where(x => x != null).Select(ConvertToNativeViewObject).ToList()
          : new List<IViewObject>(),
        obj.ViewName,
        obj.ViewId
      );
    }

    private IViewObject LayoutToNative(IViewerLayout obj)
    {
      return new Layout(obj.Viewers);
    }

    private IViewObject ViewerToNative(IViewer<VS.Layout> o)
    {
      return new Viewer(o.Layouts.Where(x => x != null).Select(LayoutToNative).Cast<IViewerLayout>().ToList());
    }

    private IViewObject ViewerToNative(IViewerLinked<VS.Layout> o)
    {
      return new ViewerLinked(o.Layouts.Where(x => x != null).Select(LayoutToNative).Cast<IViewerLayout>().ToList(), o.Clouds);
    }

    private IViewObject ResultCloudToNative(VS.ResultCloud obj)
    {
      return new ResultCloud(obj.Points, obj.Data.Where(x => x != null).Select(ResultCloudDataToNative).ToList(), obj.ViewId);
    }

    private IResultCloudData ResultCloudDataToNative(IResultCloudData obj)
    {
      return new ResultCloudData(obj.Values, obj.Option, obj.Layout);
    }

    private IViewObject ContentReferenceToNative(VS.ContentReference obj)
    {
      return new ContentReference(obj.References, obj.ContentType, obj.ViewId, obj.ViewName);
    }

    private IViewObject ViewCloudReferenceToNative(IReferenceObject obj)
    {
      return new ViewCloudReference(obj.References, obj.ViewId);
    }

    private ViewObjectReference ReferenceToNative(IReferenceObject obj)
    {
      return new ViewObjectReference(obj.References, obj.Type, obj.ViewId, obj.ViewName);
    }

    private VS.ViewStudy StudyToSpeckle(IViewStudy<IViewObject> obj)
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

    private VS.Layout LayoutToSpeckle(IViewerLayout obj)
    {
      return new VS.Layout(obj.Viewers);
    }

    private VS.Viewer ViewerToSpeckle(IViewer<IViewerLayout> o)
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
      return new VS.ResultCloudData(obj.Values, obj.Option, obj.Layout);
    }

    //TODO: Support getting list of objects from search

    private IViewObject HandleDefault(Base @base)
    {
      var obj = @base.SearchForType<VS.ViewObjectBase>(true);

      return obj == null ? null : ConvertToNativeViewObject(obj);

    }

  }
}
