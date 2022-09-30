using System.Collections.Generic;
using System.Linq;
using Speckle.Core.Models;
using ViewObjects.Cloud;
using ViewObjects.Content;
using ViewObjects.Speckle;
using ViewObjects.Study;
using ViewObjects.Viewer;
using ViewerLayout = ViewObjects.Speckle.ViewerLayout;
using ViewStudy = ViewObjects.Speckle.ViewStudy;

namespace ViewObjects.Converter
{
	/// <inheritdoc />
	public partial class ViewObjectsConverter
	{
		IViewObj StudyToNative(ViewStudy obj) => new ViewStudy_v2
		(
			obj.Objects.Valid() ? obj.Objects.Where(x => x != null).Select(ConvertToNativeViewObject).ToList()
				: new List<IViewObj>(),
			obj.ViewName,
			obj.ViewId
		);

		IViewObj ViewContentToNative(IContent obj) => new ViewContent_v2(obj.References, obj.ContentType, obj.ViewId, obj.ViewName);

		IViewObj ViewCloudToNative(IViewCloudRef_v2 obj) => new ViewCloudReference(obj.References, obj.ViewId);

		IViewObj ViewerLayoutToNative(IViewerLayout_v2 obj) => new ViewerLayout_v2(obj.Viewers);

		IViewObj ViewerSystemToNative(IViewerSystem_v2<ViewerLayout> o) =>
			new ViewerSystem_v2(o.Layouts.Where(x => x != null).Select(ViewerLayoutToNative).Cast<IViewerLayout_v2>().ToList(), o.Clouds);

		ViewObjectBase StudyToSpeckle(IViewStudy_v2<IViewObj> obj) => new ViewStudy
		{
			Objects = obj.Objects.Valid() ? obj.Objects.Where(x => x != null).Select(ConvertToSpeckleViewObject).ToList() : new List<ViewObjectBase>(),
			ViewName = obj.ViewName,
			ViewId = obj.ViewId
		};

		ViewObjectBase ViewContentToSpeckle(IContent obj) => new Speckle.Content(obj.ContentType, obj.References, obj.ViewId, obj.ViewName);

		ViewObjectBase ViewCloudToSpeckle(IViewCloudRef_v2 obj) => new ViewCloudRef(obj.References, obj.ViewId);

		ViewObjectBase ViewerLayoutToSpeckle(IViewerLayout_v2 obj) => new ViewerLayout(obj.Viewers);

		ViewObjectBase ViewerSystemToSpeckle(IViewerSystem_v2<IViewerLayout_v2> o) => new ViewerSystem(
			o.Layouts.Where(x => x != null).Select(ViewerLayoutToSpeckle).Cast<ViewerLayout>().ToList(), o.Clouds);

		//TODO: Support getting list of objects from search
		IViewObj HandleDefault(Base @base)
		{
			IViewObj o = default;

			if (@base.IsWrapper())
			{
				var obj = @base.SearchForType<ViewObjectBase>(true);

				if (obj != null) o = obj;
			}

			return o;
		}
	}
}