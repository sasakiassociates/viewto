using System.Collections.Generic;

namespace ViewObjects
{
	/// <summary>
	/// Simple Rig Object to use for constructing a study for analysis 
	/// </summary>
	public class Rig : IRig, IViewObject
	{

		/// <inheritdoc />
		public List<IViewerSystem> Viewers { get; set; }

		/// <inheritdoc />
		public List<RigParameters> Parameters { get; set; }

	}
}