using System.Collections.Generic;

namespace ViewObjects
{
	/// <summary>
	/// Simple Rig Object to use for constructing a study for analysis 
	/// </summary>
	public class Rig : IRig, IViewObject
	{

		/// <inheritdoc />
		public void Build(List<IViewerSystem> viewers)
		{ }

		/// <inheritdoc />
		public void Initialize(List<RigParameters> parameters)
		{ }
	}
}