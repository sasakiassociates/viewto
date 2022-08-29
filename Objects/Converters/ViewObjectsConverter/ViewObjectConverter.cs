using System;
using System.Collections.Generic;
using System.Linq;
using Speckle.Core.Kits;
using Speckle.Core.Models;
using ViewObjects.Speckle;

namespace ViewObjects.Converter.Script
{

	public partial class ViewObjectConverter : ISpeckleConverter
	{

		public ViewObjectSchema Schema;

		// TODO: Check if this Kit and Converter are installed
		public ISpeckleConverter supportConverter { get; protected set; }

		public ViewObjectConverter()
		{
			Schema = new ViewObjectSchema();
		}

		public ViewObjectConverter(ViewObjectSchema schema, ISpeckleConverter converter)
		{
			Schema = schema;
			supportConverter = converter;
		}

		public virtual List<ApplicationPlaceholderObject> ContextObjects { get; set; } = new List<ApplicationPlaceholderObject>();

		public virtual string Description => "Converter for basic view objects";

		public virtual string Name => nameof(ViewObjectConverter);

		public virtual string Author => "David Morgan";

		public virtual string WebsiteOrEmail => "https://sasaki.com";

		public virtual HashSet<Exception> ConversionErrors { get; } = new HashSet<Exception>();

		public ProgressReport Report { get; } = new ProgressReport();

		public ReceiveMode ReceiveMode { get; set; }

		public virtual IEnumerable<string> GetServicedApplications()
		{
			return new[]
			{
				VersionedHostApplications.Script
			};
		}

		public virtual void SetContextDocument(object doc)
		{ }

		public virtual void SetContextObjects(List<ApplicationPlaceholderObject> objects) => ContextObjects = objects;

		public virtual void SetPreviousContextObjects(List<ApplicationPlaceholderObject> objects)
		{ }

		public void SetConverterSettings(object settings) => Console.WriteLine(settings);

		public List<object> ConvertToNative(List<Base> objects) => objects.Select(ConvertToNative).ToList();

		public object ConvertToNative(Base @base) => ConvertToViewObj(@base);

		public Base ConvertToSpeckle(object @object) => ConvertToViewObjBase(@object);

		public List<Base> ConvertToSpeckle(List<object> objects) => objects.Select(ConvertToSpeckle).ToList();

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

		public IViewObj ConvertToViewObj(Base @base)
		{
			switch (@base)
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
					throw new ArgumentOutOfRangeException(nameof(@base), @base, null);
			}
		}

		public ViewObjBase ConvertToViewObjBase(object @object)
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
	}
}