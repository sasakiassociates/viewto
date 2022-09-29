using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace ViewTo.RhinoGh
{
	public class ViewToRhinoGhInfo : GH_AssemblyInfo
	{
		public override string Name
		{
			get => "ViewToRhinoGh";
		}

		public override Bitmap Icon
		{
			get =>
				//Return a 24x24 pixel bitmap to represent this GHA library.
				null;
		}

		public override string Description
		{
			get => "A View Analysis Thing";
		}

		public override Guid Id
		{
			get => new Guid("b8bd4fc4-1553-4601-acdd-ad45382f7196");
		}

		public override string AuthorName
		{
			get => "Sasaki";
		}

		public override string AuthorContact
		{
			get => "dmorgan@sasaki.com";
		}
	}
}