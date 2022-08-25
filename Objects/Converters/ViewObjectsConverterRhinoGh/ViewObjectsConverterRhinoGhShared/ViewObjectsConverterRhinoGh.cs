using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using Speckle.Core.Kits;
using Speckle.Core.Models;
using ViewObjects.Converter.Script;
using ViewObjects.Speckle;

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

		
		protected override ViewContentBase ViewContentToSpeckle(IViewContent content)
		{
			var @base = base.ViewContentToSpeckle(content);
			if (@base == null ) return null;

			if (supportConverter == null) return @base; // check if dll loaded

			Mesh m = null;
			
			if (content.objects[0] is Mesh) // if already converted
			{
				@base.objects = content.objects.Select(o => supportConverter.ConvertToSpeckle(o)).Where(i => i != null).ToList();
			}
			else // try converting from GH_Mesh
			{
				var meshes = (from i in content.objects where GH_Convert.ToMesh(i, ref m, GH_Conversion.Primary) select m).ToList();
				@base.objects = meshes.Select(o => supportConverter.ConvertToSpeckle(o)).Where(i => i != null).ToList();
			}

			return @base;
		}
	}
}