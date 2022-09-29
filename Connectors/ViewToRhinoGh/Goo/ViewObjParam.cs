using System;
using Grasshopper.Kernel;

namespace ViewTo.RhinoGh.Goo
{

	public class ViewObjParam : GH_Param<GH_ViewObj>
	{

		public ViewObjParam(string name, string nickname, string description, GH_ParamAccess access) :
			this(name, nickname, description, ConnectorInfo.CATEGORY, "Params", access)
		{ }

		public ViewObjParam(string name, string nickname, string description, string category, string subcategory, GH_ParamAccess access) : base(
			name, nickname, description, category, subcategory, access)
		{ }

		public ViewObjParam() : this("ViewObj", "V", "View Object for View To", GH_ParamAccess.item)
		{ }

		public override Guid ComponentGuid
		{
			get => new Guid("97D2B0E4-5B1B-4BAB-9D25-FDC2D69E510D");
		}

		public override GH_Exposure Exposure
		{
			get => GH_Exposure.hidden;
		}
	}

}