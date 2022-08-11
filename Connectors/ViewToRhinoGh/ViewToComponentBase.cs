using System;
using Grasshopper.Kernel;

namespace ViewTo.RhinoGh
{
	public abstract class ViewToComponentBase : GH_Component
	{

		public ViewToComponentBase(string name, string nickname, string description, string sub) : base(
			name, nickname, description, ConnectorInfo.CATEGORY, sub)
		{ }

		
	}
}