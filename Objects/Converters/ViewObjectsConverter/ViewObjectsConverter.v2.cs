using System.Collections.Generic;
using System.Linq;
using Speckle.Core.Models;
using ViewObjects.References;
using VS = ViewObjects.Speckle;
using VO = ViewObjects;

namespace ViewObjects.Converter
{
	/// <inheritdoc />
	public partial class ViewObjectsConverter
	{
		IViewObject StudyToNative(VS.ViewStudy obj) => new Study.ViewStudy
		(
			obj.Objects.Valid() ? obj.Objects.Where(x => x != null).Select(ConvertToNativeViewObject).ToList()
				: new List<IViewObject>(),
			obj.ViewName,
			obj.ViewId
		);

		IViewObject ViewContentToNative(VS.Content obj) => new ContentReference(obj.References, obj.ContentType, obj.ViewId, obj.ViewName);

		IViewObject ViewCloudToNative(IReferenceObject obj) => new CloudReference(obj.References, obj.ViewId);

		IViewObject ViewerLayoutToNative(IViewerLayout obj) => new Viewer.ViewerLayout(obj.Viewers);

		IViewObject ViewerSystemToNative(IViewerSystem<VS.ViewerLayout> o) =>
			new Viewer.ViewerSystem(o.Layouts.Where(x => x != null).Select(ViewerLayoutToNative).Cast<IViewerLayout>().ToList(), o.Clouds);

		VS.ViewObjectBase StudyToSpeckle(IViewStudy<IViewObject> obj) => new VS.ViewStudy
		{
			Objects = obj.Objects.Valid() ? obj.Objects.Where(x => x != null).Select(ConvertToSpeckleViewObject).ToList() : new List<VS.ViewObjectBase>(),
			ViewName = obj.ViewName,
			ViewId = obj.ViewId
		};

		VS.ViewObjectBase ViewContentToSpeckle(ContentReference obj) => new VS.Content(obj.ContentType, obj.References, obj.ViewId, obj.ViewName);

		VS.ViewObjectBase ViewCloudToSpeckle(IReferenceObject obj) => new VS.ViewCloud(obj.References, obj.ViewId);

		VS.ViewObjectBase ViewObjectReferenceToSpeckle(IReferenceObject obj) =>
			new VS.ViewObjectReferenceBase(obj.References, obj.Type, obj.ViewId, obj.ViewName);

		VS.ViewObjectBase ViewerLayoutToSpeckle(IViewerLayout obj) => new VS.ViewerLayout(obj.Viewers);

		VS.ViewObjectBase ViewerSystemToSpeckle(IViewerSystem<IViewerLayout> o) => new VS.ViewerSystem(
			o.Layouts.Where(x => x != null).Select(ViewerLayoutToSpeckle).Cast<VS.ViewerLayout>().ToList(), o.Clouds);

		//TODO: Support getting list of objects from search
		IViewObject HandleDefault(Base @base)
		{
			IViewObject o = default;

			if (@base.IsWrapper())
			{
				var obj = @base.SearchForType<VS.ViewObjectBase>(true);

				if (obj != null)
					o = obj;
			}

			return o;
		}
	}
}