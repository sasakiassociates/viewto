using System;
using System.Collections.Generic;
using System.Linq;
using Speckle.Core.Kits;
using Speckle.Core.Models;
using ViewObjects.Speckle;

namespace ViewObjects.Converter
{
	public partial class ViewObjectsConverter : ISpeckleConverter
	{

		public IViewObjectFactory Schema { get; set; } = new ViewObjectFactory();

		public virtual string Name => nameof(ViewObjectsConverter);

		public virtual string Description => "Converter for basic view objects";

		public virtual string Author => "David Morgan";

		public virtual string WebsiteOrEmail => "https://sasaki.com";

		public virtual ProgressReport Report { get; }

		public virtual ReceiveMode ReceiveMode { get; set; }

		public virtual IEnumerable<string> GetServicedApplications()
		{
			return new[]
			{
				VersionedHostApplications.Grasshopper6,
				VersionedHostApplications.Grasshopper7,
				VersionedHostApplications.Rhino6,
				VersionedHostApplications.Rhino7
			};
		}

		public virtual void SetContextDocument(object doc)
		{ }

		public virtual void SetContextObjects(List<ApplicationObject> objects)
		{ }

		public virtual void SetPreviousContextObjects(List<ApplicationObject> objects)
		{ }

		public virtual void SetConverterSettings(object settings)
		{ }

		public List<Base> ConvertToSpeckle(List<object> objects) => objects.Select(ConvertToSpeckle).ToList();

		public Base ConvertToSpeckle(object @object) => ConvertToSpeckleViewObject(@object);

		public ViewObjectBase_v2 ConvertToSpeckleViewObject(object @object)
		{
			switch (@object)
			{
				case IViewStudy_v2<IViewObj> o:
					return StudyToSpeckle(o);
				case IViewCloudRef_v2 o:
					return ViewCloudToSpeckle(o);
				case IContent o:
					return ViewContentToSpeckle(o);
				case IViewerLayout_v2 o:
					return ViewerLayoutToSpeckle(o);
				case IViewerSystem_v2<IViewerLayout_v2> o:
					return ViewerSystemToSpeckle(o);
				default:
					throw new ArgumentOutOfRangeException(nameof(@object), @object, null);
			}
		}

		public List<object> ConvertToNative(List<Base> objects) => objects.Select(ConvertToNative).ToList();

		public object ConvertToNative(Base @object) => ConvertToNativeViewObject(@object);

		public IViewObj ConvertToNativeViewObject(Base @object)
		{
			switch (@object)
			{
				case ViewStudyBase_v2 o:
					return StudyToNative(o);
				case ViewCloudShellBaseV2 o:
					return ViewCloudToNative(o);
				case ContentBase_v2 o:
					return ViewContentToNative(o);
				case ViewerLayoutBase_v2 o:
					return ViewerLayoutToNative(o);
				case ViewerSystemBase_v2 o:
					return ViewerSystemToNative(o);
				case Base o:
					return HandleDefault(o);
				default:
					throw new ArgumentOutOfRangeException(nameof(@object), @object, null);
			}
		}

		public bool CanConvertToSpeckle(object @object)
		{
			switch (@object)
			{
				case IViewStudy _:
					return true;
				case IResultCloud _:
					return true;
				case IViewCloud _:
					return true;
				case IViewContent _:
					return true;
				case IViewContentBundle _:
					return true;
				case IViewerBundle _:
					return true;
				case IViewerLayout _:
					return true;
				// V2 objects
				case IViewStudy_v2<IViewObj> _:
					return true;
				case IViewCloudRef_v2 _:
					return true;
				case IContent _:
					return true;
				case IViewerLayout_v2 _:
					return true;
				case IViewerSystem_v2<IViewerLayout_v2> _:
					return true;
				default:
					return false;
			}
		}

		public bool CanConvertToNative(Base @base)
		{
			switch (@base)
			{
				case ViewStudyBaseV1 _:
					return true;
				case ResultCloudBaseV1 _:
					return true;
				case ViewCloudBaseV1 _:
					return true;
				case ContentBundleBaseV1 _:
					return true;
				case ViewerBundleBaseV1 _:
					return true;
				case ViewerLayoutBaseV1 _:
					return true;
				// V2 objects
				case ViewStudyBase_v2 _:
					return true;
				case ViewCloudShellBaseV2 _:
					return true;
				case ContentBase_v2 _:
					return true;
				case ViewerLayoutBase_v2 _:
					return true;
				case ViewerSystemBase_v2 _:
					return true;
				default:
					return false;
			}
		}

	}
}