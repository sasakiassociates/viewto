using System.Collections.Generic;
using Speckle.Core.Kits;
using Speckle.Core.Models;

namespace ViewObjects.Converter
{
	public class ViewToConverter : ISpeckleConverter
	{

		public Base ConvertToSpeckle(object @object) => throw new System.NotImplementedException();

		public List<Base> ConvertToSpeckle(List<object> objects) => throw new System.NotImplementedException();

		public bool CanConvertToSpeckle(object @object) => throw new System.NotImplementedException();

		public object ConvertToNative(Base @object) => throw new System.NotImplementedException();

		public List<object> ConvertToNative(List<Base> objects) => throw new System.NotImplementedException();

		public bool CanConvertToNative(Base @object) => throw new System.NotImplementedException();

		public IEnumerable<string> GetServicedApplications() => throw new System.NotImplementedException();

		public void SetContextDocument(object doc)
		{
			throw new System.NotImplementedException();
		}

		public void SetContextObjects(List<ApplicationPlaceholderObject> objects)
		{
			throw new System.NotImplementedException();
		}

		public void SetPreviousContextObjects(List<ApplicationPlaceholderObject> objects)
		{
			throw new System.NotImplementedException();
		}

		public void SetConverterSettings(object settings)
		{
			throw new System.NotImplementedException();
		}

		public string Description { get; }

		public string Name { get; }

		public string Author { get; }

		public string WebsiteOrEmail { get; }

		public ProgressReport Report { get; }

		public ReceiveMode ReceiveMode { get; set; }
	}
}