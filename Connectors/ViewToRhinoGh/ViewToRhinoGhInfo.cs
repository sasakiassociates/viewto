using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace ViewTo.RhinoGh
{
	public class ViewToRhinoGhInfo : GH_AssemblyInfo
	{
		public override string Name => "ViewToRhinoGh";

		public override Bitmap Icon
		{
			get
			{
				//Return a 24x24 pixel bitmap to represent this GHA library.
				return null;
			}
		}

		public override string Description => "A View Analysis Thing";

		public override Guid Id => new Guid("b8bd4fc4-1553-4601-acdd-ad45382f7196");

		public override string AuthorName => "Sasaki";

		public override string AuthorContact => "dmorgan@sasaki.com";
	}
}