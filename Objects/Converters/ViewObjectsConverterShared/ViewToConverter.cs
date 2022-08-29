using System;
using System.Collections.Generic;
using System.Linq;
using Speckle.Core.Kits;
using Speckle.Core.Models;
using ViewObjects.Speckle;

namespace ViewObjects.Converter
{
	public abstract class ViewToConverter : ISpeckleConverter
	{
		public virtual string Name => nameof(ViewToConverter);

		public virtual string Description => "Converter for basic view objects";

		public virtual string Author => "Sasaki-Strategies";

		public virtual string WebsiteOrEmail => "https://sasaki.com";

		public abstract ProgressReport Report { get; }

		public abstract ReceiveMode ReceiveMode { get; set; }

		public List<Base> ConvertToSpeckle(List<object> objects) => objects.Select(ConvertToSpeckle).ToList();

		public Base ConvertToSpeckle(object @object)
		{
			switch (@object)
			{
				case IViewStudy o:
					return StudyToSpeckle(o);
				case IResultCloud o:
					return ResultCloudToSpeckle(o);
				case IViewCloud o:
					return ViewCloudToSpeckle(o);
				case IViewContentBundle o:
					return ContentBundleToSpeckle(o);
				case IViewContent o:
					return ViewContentToSpeckle(o);
				case IViewerBundle o:
					return ViewerBundleToSpeckle(o);
				case IViewerLayout o:
					return LayoutToSpeckle(o);
				default:
					throw new ArgumentOutOfRangeException(nameof(@object), @object, null);
			}
		}

		public List<object> ConvertToNative(List<Base> objects) => objects.Select(ConvertToNative).ToList();

		public object ConvertToNative(Base @object)
		{
			switch (@object)
			{
				case ViewStudyBase o:
					return StudyToNative(o);
				case ResultCloudBase o:
					return ResultCloudToNative(o);
				case ViewCloudBase o:
					return ViewCloudToNative(o);
				case ContentBundleBase o:
					return ContentBundleToNative(o);
				case ViewContentBase o:
					return ViewContentToNative(o);
				case ViewerBundleBase o:
					return ViewerBundleToNative(o);
				case ViewerLayoutBase o:
					return LayoutToNative(o);
				case Base o:
					return HandleDefault(o);
				default:
					throw new ArgumentOutOfRangeException(nameof(@object), @object, null);
			}
		}

		public bool CanConvertToNative(Base @base)
		{
			switch (@base)
			{
				case ViewStudyBase _:
					return true;
				case ResultCloudBase _:
					return true;
				case ViewCloudBase _:
					return true;
				case ContentBundleBase _:
					return true;
				case ViewerBundleBase _:
					return true;
				case ViewerLayoutBase _:
					return true;
				default:
					return false;
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
				default:
					return false;
			}
		}

		public abstract IEnumerable<string> GetServicedApplications();

		public abstract void SetContextDocument(object doc);

		public abstract void SetContextObjects(List<ApplicationPlaceholderObject> objects);

		public abstract void SetPreviousContextObjects(List<ApplicationPlaceholderObject> objects);

		public abstract void SetConverterSettings(object settings);

	}
}