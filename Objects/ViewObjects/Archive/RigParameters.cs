using System.Collections.Generic;

namespace ViewObjects
{
	public class RigParameters_V1 : IRigParam_v1
	{
		/// <summary>
		///   List of bundles for different viewer objects
		/// </summary>
		public List<IViewerBundle_v1> bundles { get; set; }
	}

	public class RigParametersIsolated : RigParameters_V1
	{
		public List<ViewColor> colors { set; get; }
	}
}