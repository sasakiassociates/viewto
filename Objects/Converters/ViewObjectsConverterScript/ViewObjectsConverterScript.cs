using System;
using System.Collections.Generic;
using Speckle.Core.Kits;
using Speckle.Core.Models;

namespace ViewObjects.Converter.Script
{
	public class ViewObjectsConverterScript : ViewObjectsConverter
	{
		public ViewObjectsConverterScript()
		{
			Schema = new ViewObjectSchema();
		}

		public override ProgressReport Report { get; }

		public override ReceiveMode ReceiveMode { get; set; }

		public override IEnumerable<string> GetServicedApplications() => new[] { VersionedHostApplications.Script };

		public override void SetContextDocument(object doc)
		{
			throw new NotImplementedException();
		}

		public override void SetContextObjects(List<ApplicationPlaceholderObject> objects)
		{
			throw new NotImplementedException();
		}

		public override void SetPreviousContextObjects(List<ApplicationPlaceholderObject> objects)
		{
			throw new NotImplementedException();
		}

		public override void SetConverterSettings(object settings)
		{
			throw new NotImplementedException();
		}

		protected override bool ViewContentDataToSpeckle(List<object> items, out List<Base> result)
		{
			result = new List<Base>();
			if (!items.Valid()) return false;

			foreach (var item in items)
			{
				Base obj = null;

				if (CanConvertToSpeckle(item))
					obj = ConvertToSpeckle(item);

				if (obj == null)
					continue;

				result.Add(obj);
			}

			return result.Valid();
		}

		protected override bool ViewContentDataToNative(List<Base> items, out List<object> result)
		{
			result = new List<object>();

			if (!items.Valid()) return false;

			foreach (var item in items)
			{
				object obj = null;

				if (CanConvertToSpeckle(item))
					obj = ConvertToSpeckle(item);

				if (obj == null)
					continue;

				result.Add(obj);
			}

			return result.Valid();
		}
	}
}