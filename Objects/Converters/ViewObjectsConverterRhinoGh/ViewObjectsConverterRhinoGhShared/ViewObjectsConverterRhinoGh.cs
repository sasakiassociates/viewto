using Rhino;
using Speckle.Core.Kits;

namespace ViewObjects.Converter.Rhino
{
	public class ViewObjRhinoConverter : ViewObjectsConverter
	{
		public ViewObjRhinoConverter() => Schema = new ViewObjectFactory();

		#if RHINO6 && GRASSHOPPER
		public static string RhinoAppName = VersionedHostApplications.Grasshopper6;
		#elif RHINO7 && GRASSHOPPER
		public static string RhinoAppName = VersionedHostApplications.Grasshopper7;
		#elif RHINO6
		public static string RhinoAppName = VersionedHostApplications.Rhino6;
		#elif RHINO7
		public static string RhinoAppName = VersionedHostApplications.Rhino7;
		#endif

		public override string Name
		{
			get => nameof(ViewObjRhinoConverter);
		}

		public override string Description
		{
			get => "Converter for rhino/gh objects into base view objects";
		}

		// public override IEnumerable<string> GetServicedApplications()
		// {
		// 	return new[] { RhinoAppName };
		// }

		/// <summary>
		///   Current way of attaching default object kit to this converter
		/// </summary>
		/// <param name="doc"></param>
		public override void SetContextDocument(object doc)
		{
			Doc = (RhinoDoc)doc;
		}

		public RhinoDoc Doc { get; set; }

	}
}