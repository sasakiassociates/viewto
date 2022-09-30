using System;
using System.Collections.Generic;
using System.Linq;
using Speckle.Core.Kits;
using Speckle.Core.Models;
using ViewObjects.References;
using VS = ViewObjects.Speckle;
using VO = ViewObjects;

namespace ViewObjects.Converter
{
	public partial class ViewObjectsConverter : ISpeckleConverter
	{

		public IViewObjectFactory Schema { get; set; } = new ViewObjectFactory();

		public virtual string Name
		{
			get => nameof(ViewObjectsConverter);
		}

		public virtual string Description
		{
			get => "Converter for basic view objects";
		}

		public virtual string Author
		{
			get => "David Morgan";
		}

		public virtual string WebsiteOrEmail
		{
			get => "https://sasaki.com";
		}

		public virtual ProgressReport Report { get; set; }

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

		public List<object> ConvertToNative(List<Base> objects) => objects.Select(ConvertToNative).ToList();

		public object ConvertToNative(Base @object) => ConvertToNativeViewObject(@object);

		public bool CanConvertToSpeckle(object @object)
		{
			switch (@object)
			{
				case IReferenceObject _:
					return true;
				case IViewStudy<IViewObject> _:
					return true;
				case IResultCloud _:
					return true;
				case ICloud _:
					return true;
				case IContent _:
					return true;
				case IViewerLayout _:
					return true;
				case IViewerSystem<IViewerLayout> _:
					return true;
				default:
					return false;
			}
		}

		public bool CanConvertToNative(Base @base)
		{
			switch (@base)
			{
				// V2 objects
				case VS.ViewStudy _:
					return true;
				case VS.ViewCloud _:
					return true;
				case VS.Content _:
					return true;
				case VS.ViewerLayout _:
					return true;
				case VS.ViewerSystem _:
					return true;
				case VS.ResultCloud _:
					return true;
				case VS.ResultCloudData _:
					return true;
				default:
					return false;
			}
		}

		public VS.ViewObjectBase ConvertToSpeckleViewObject(object @object)
		{
			switch (@object)
			{
				case IViewStudy<IViewObject> o:
					return StudyToSpeckle(o);
				case IViewerLayout o:
					return ViewerLayoutToSpeckle(o);
				case IViewerSystem<IViewerLayout> o:
					return ViewerSystemToSpeckle(o);
				case IResultCloud o:
					return ResultCloudToSpeckle(o);
				case IResultCloudData o:
					return ResultCloudDataToSpeckle(o);
				case CloudReference o:
					return ViewCloudToSpeckle(o);
				case ContentReference o:
					return ViewContentToSpeckle(o);
				case ViewObjectReference o:
					return ViewObjectReferenceToSpeckle(o);
				default:
					throw new ArgumentOutOfRangeException(nameof(@object), @object, null);
			}
		}

		public IViewObject ConvertToNativeViewObject(Base @object)
		{
			switch (@object)
			{
				case VS.ViewStudy o:
					return StudyToNative(o);
				case VS.ViewCloud o:
					return ViewCloudToNative(o);
				case VS.Content o:
					return ViewContentToNative(o);
				case VS.ViewerLayout o:
					return ViewerLayoutToNative(o);
				case VS.ViewerSystem o:
					return ViewerSystemToNative(o);
				case VS.ResultCloud o:
					return ResultCloudToNative(o);
				case Base o:
					return HandleDefault(o);
				default:
					throw new ArgumentOutOfRangeException(nameof(@object), @object, null);
			}
		}
	}
}