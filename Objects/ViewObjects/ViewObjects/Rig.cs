using System;
using System.Collections.Generic;

namespace ViewObjects
{
	/// <summary>
	/// Simple Rig Object to use for constructing a study for analysis 
	/// </summary>
	public class Rig : IRig, IViewObject
	{

		public List<RigParameters> StoredObjs { get; protected set; }

		public Rig()
		{ }

		/// <inheritdoc />
		public void Build()
		{
			Console.WriteLine("Building Rig");
		}

		/// <inheritdoc />
		public void Initialize(List<RigParameters> parameters)
		{
			StoredObjs = parameters;
		}
	}
}