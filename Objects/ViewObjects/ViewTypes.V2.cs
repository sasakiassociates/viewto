using System;
using System.Collections.Generic;

namespace ViewObjects
{

	[Serializable]
	public class RigParamArgsV2
	{
		public RigParamArgsV2(List<IViewerBundle> bundles, List<ViewColor> colors)
		{
			this.bundles = bundles;
			this.colors = colors;
		}

		List<IViewerBundle> bundles { get; }

		List<ViewColor> colors { get; }
	}

	// public interface IRig_v2
	// {
	// 	public void Load(List<RigArg> args);
	// }
	//
	// public class RigArg
	// {
	// 	/// <summary>
	// 	/// List of different bundles to use 
	// 	/// </summary>
	// 	List<IViewerSystem_v2> bundles { get; set; }
	//
	// 	/// <summary>
	// 	/// List of colors to use 
	// 	/// </summary>
	// 	List<ViewColor> colors { get; set; }
	// }

}