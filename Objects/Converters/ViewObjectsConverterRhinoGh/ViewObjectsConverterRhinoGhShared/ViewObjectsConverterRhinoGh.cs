using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino;
using Rhino.Geometry;
using Speckle.Core.Kits;
using Speckle.Core.Models;
using ViewObjects.Converter.Script;

namespace ViewObjects.Converter.Rhino
{
	public partial class ViewObjRhinoConverter : ViewObjectConverter
	{
		public ViewObjRhinoConverter()
		{
			Schema = new ViewObjectSchema();
			supportConverter = KitManager.GetDefaultKit().LoadConverter(RhinoAppName);
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

		public override string Description => "Converter for rhino/gh objects into base view objects";

		public override string Name => nameof(ViewObjRhinoConverter);

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
			supportConverter?.SetContextDocument(Doc);
		}

		protected override bool ViewContentDataToSpeckle(List<object> items, out List<Base> result)
		{
			result = new List<Base>();

			if (items == null || supportConverter == null) return false;

			foreach (var item in items)
			{
				Base b = null;

				if (supportConverter.CanConvertToSpeckle(item))
				{
					b = supportConverter.ConvertToSpeckle(item);
				}
				else if (item is GH_Mesh gm)
				{
					Mesh m = null;
					GH_Convert.ToMesh(gm, ref m, GH_Conversion.Primary);
					b = supportConverter.ConvertToSpeckle(m);
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

	}
}