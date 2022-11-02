using Rhino;
using Speckle.Core.Kits;
namespace ViewObjects.Converter.Rhino
{
  public class ViewObjRhinoConverter : ViewObjectsConverter
  {

		#if RHINO6 && GRASSHOPPER
    public static string RhinoAppName = HostApplications.Grasshopper.GetVersion(HostAppVersion.v6);
		#elif RHINO7 && GRASSHOPPER
    public static string RhinoAppName = HostApplications.Grasshopper.GetVersion(HostAppVersion.v7);
		#elif RHINO6
    public static string RhinoAppName = HostApplications.Rhino.GetVersion(HostAppVersion.v6);
		#elif RHINO7
    public static string RhinoAppName = HostApplications.Rhino.GetVersion(HostAppVersion.v7);
		#endif

    public override string Name => nameof(ViewObjRhinoConverter);

    public override string Description => "Converter for rhino/gh objects into base view objects";

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
