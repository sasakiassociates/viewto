using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino;
using Rhino.Geometry;
using ViewObjects;
using ViewObjects.Cloud;

namespace ViewTo.RhinoGh
{

	public class ConnectorPipe
	{
		public static bool isInit;

		private static RhinoDoc doc { get; set; }

		public static void SetDoc(RhinoDoc activeDoc)
		{
			doc = activeDoc;
			// Init();
		}

		public static ViewCloud Prime(IEnumerable<GH_Point> points)
		{
			// Init();

			var p = new Point3d();
			return new ViewCloud
			{
				points = (from t in points where GH_Convert.ToPoint3d(t, ref p, GH_Conversion.Primary) select new CloudPoint
				{
					x = p.X, y = p.Y, z = p.Z
				}).ToArray()
			};
		}

		public static TContent Prime<TContent>(IEnumerable<GH_Mesh> meshes, GH_String name) where TContent : IViewContent, new()
		{
			// Init();

			Mesh m = null;
			GH_Convert.ToString(name, out var viewName, GH_Conversion.Both);
			return new TContent
			{
				objects = (from i in meshes where GH_Convert.ToMesh(i, ref m, GH_Conversion.Primary) select m).Cast<object>().ToList(), viewName = viewName
			};
		}
	}
}