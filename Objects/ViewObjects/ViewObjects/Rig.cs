using System;
using System.Collections.Generic;

namespace ViewObjects
{
	/// <summary>
	/// Simple Rig Object to use for constructing a study for analysis 
	/// </summary>
	public class Rig : IRig, IViewObject
	{

		public List<IViewer> Viewers { get; protected set; }

		public Rig()
		{ }

		/// <inheritdoc />
		public void Build()
		{ }

		/// <inheritdoc />
		public void Initialize(List<RigParameters> parameters)
		{
			foreach (var p in parameters)
			{
				Activator.CreateInstance<Viewer>();
			}
		}
	}
}