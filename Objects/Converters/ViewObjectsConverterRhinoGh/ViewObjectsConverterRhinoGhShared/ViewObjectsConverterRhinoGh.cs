using System.Collections.Generic;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using Speckle.Core.Kits;
using Speckle.Core.Models;
using ViewObjects.Converter.Script;

namespace ViewObjects.Converter.Rhino
{
	public partial class ViewObjRhinoConverter : ViewObjectConverter
	{

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

		private ISpeckleConverter defaultConverter { get; set; }

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
			defaultConverter = KitManager.GetDefaultKit().LoadConverter(RhinoAppName);
			defaultConverter?.SetContextDocument(Doc);
		}

		public override object ConvertToNative(Base @base) => base.ConvertToNative(@base);
	}
}