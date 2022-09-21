using System.Collections.Generic;
using System.Linq;
using ViewObjects.Cloud;
using ViewObjects.Content;
using ViewObjects.Speckle;
using ViewObjects.Study;
using ViewObjects.Viewer;

namespace ViewObjects.Converter
{
	public abstract partial class ViewObjectsConverter
	{
		IViewObj StudyToNative(ViewStudyBase_v2 obj) => new ViewStudy_v2
		(
			obj.Objects.Valid() ? obj.Objects.Where(x => x != null).Select(ConvertToNativeViewObject).ToList()
				: new List<IViewObj>(),
			obj.ViewName,
			obj.ViewId
		);

		IViewObj ViewContentToNative(IContent obj) => new ViewContent_v2(obj.References, obj.ContentType, obj.ViewId, obj.ViewName);

		IViewObj ViewCloudToNative(IViewCloud_v2 obj) => new ViewCloudReference(obj.References, obj.ViewId);

		IViewObj ViewerLayoutToNative(IViewerLayout_v2 obj) => new ViewerLayout_v2(obj.Viewers);

		IViewObj ViewerSystemToNative(IViewerSystem_v2<ViewerLayoutBase_v2> o) =>
			new ViewerSystem_v2(o.Layouts.Where(x => x != null).Select(ViewerLayoutToNative).Cast<IViewerLayout_v2>().ToList(), o.Clouds);

		ViewObjectBase_v2 StudyToSpeckle(IViewStudy_v2<IViewObj> obj) => new ViewStudyBase_v2()
		{
			Objects = obj.Objects.Valid() ? obj.Objects.Where(x => x != null).Select(ConvertToSpeckleViewObject).ToList() : new List<ViewObjectBase_v2>(),
			ViewName = obj.ViewName,
			ViewId = obj.ViewId
		};

		ViewObjectBase_v2 ViewContentToSpeckle(IContent obj) => new ContentBase_v2(obj.ContentType, obj.References, obj.ViewId, obj.ViewName);

		ViewObjectBase_v2 ViewCloudToSpeckle(IViewCloud_v2 obj) => new ViewCloudBase_v2(obj.References, obj.ViewId);

		ViewObjectBase_v2 ViewerLayoutToSpeckle(IViewerLayout_v2 obj) => new ViewerLayoutBase_v2(obj.Viewers);

		ViewObjectBase_v2 ViewerSystemToSpeckle(IViewerSystem_v2<IViewerLayout_v2> o) => new ViewerSystemBase_v2(
			o.Layouts.Where(x => x != null).Select(ViewerLayoutToSpeckle).Cast<ViewerLayoutBase_v2>().ToList(), o.Clouds);
	}
}