using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino;
using Rhino.Geometry;
using Speckle.Core.Kits;
using Speckle.Core.Models;

namespace ViewObjects.Converter.Rhino
{
	public class ViewObjRhinoConverter : ViewToConverter
	{
		public ViewObjRhinoConverter()
		{
			Schema = new ViewObjectSchema();
			SupportConverter = KitManager.GetDefaultKit().LoadConverter(RhinoAppName);
		}

		#if RHINO6 && GRASSHOPPER
		public static string RhinoAppName = VersionedHostApplications.Grasshopper6;
		#elif RHINO7 && GRASSHOPPER
		public static string RhinoAppName = VersionedHostApplications.Grasshopper7;
		#elif RHINO6
		public static string RhinoAppName = VersionedHostApplications.Rhino6;
		#elif RHINO7
		public static string RhinoAppName = VersionedHostApplications.Rhino7;
		#endif

		public override string Name => nameof(ViewObjRhinoConverter);

		public override string Description => "Converter for rhino/gh objects into base view objects";

		public override ProgressReport Report { get; }

		public override ReceiveMode ReceiveMode { get; set; }

		public sealed override ISpeckleConverter SupportConverter { get; set; }

		public RhinoDoc Doc { get; set; }

		public override IEnumerable<string> GetServicedApplications()
		{
			return new[] { RhinoAppName };
		}

		/// <summary>
		///   Current way of attaching default object kit to this converter
		/// </summary>
		/// <param name="doc"></param>
		public override void SetContextDocument(object doc)
		{
			Doc = (RhinoDoc)doc;
			SupportConverter?.SetContextDocument(Doc);
		}

		public override void SetContextObjects(List<ApplicationPlaceholderObject> objects) => SupportConverter?.SetContextObjects(objects);

		public override void SetPreviousContextObjects(List<ApplicationPlaceholderObject> objects) => SupportConverter?.SetPreviousContextObjects(objects);

		public override void SetConverterSettings(object settings) => SupportConverter?.SetConverterSettings(settings);

		protected override bool ViewContentDataToSpeckle(List<object> items, out List<Base> result)
		{
			result = new List<Base>();

			if (items == null || SupportConverter == null) return false;

			foreach (var item in items)
			{
				Base b = null;

				if (SupportConverter.CanConvertToSpeckle(item))
				{
					b = SupportConverter.ConvertToSpeckle(item);
				}
				else if (item is GH_Mesh gm)
				{
					Mesh m = null;
					GH_Convert.ToMesh(gm, ref m, GH_Conversion.Primary);
					b = SupportConverter.ConvertToSpeckle(m);
				}
				else
				{
					Console.WriteLine($"Not converted {item}");
				}

				if (b == null)
				{
					Console.WriteLine($"Object was not converted properly");
					continue;
				}

				result.Add(b);
			}

			return result.Valid();
		}

		protected override bool ViewContentDataToNative(List<Base> items, out List<object> result)
		{
			result = new List<object>();

			if (items == null || SupportConverter == null) return false;

			foreach (var item in items)
			{
				object obj = null;

				if (SupportConverter.CanConvertToNative(item))
					obj = SupportConverter.ConvertToNative(item);
				else if (CanConvertToNative(item))
					obj = ConvertToNative(item);
				else
					Console.WriteLine($"Not converted {item}");

				if (obj == null)
				{
					Console.WriteLine($"Object was not converted properly");
					continue;
				}

				result.Add(obj);
			}

			return result.Valid();
		}

	}
}