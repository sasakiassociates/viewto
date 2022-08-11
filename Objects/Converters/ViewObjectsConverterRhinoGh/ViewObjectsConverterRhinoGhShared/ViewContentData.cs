using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;
using ViewObjects.Speckle;

namespace ViewObjects.Converter.Rhino
{
	public partial class ViewObjRhinoConverter
	{
		protected override ViewContentBase ViewContentToSpeckle(IViewContent content)
		{
			var @base = base.ViewContentToSpeckle(content);
			if (@base == null) return null;

			if (defaultConverter == null) return @base; // check if dll loaded

			Mesh m = null;

			if (content.objects[0] is Mesh) // if already converted
			{
				@base.objects = content.objects.Select(o => defaultConverter.ConvertToSpeckle(o)).Where(i => i != null).ToList();
			}
			else // try converting from GH_Mesh
			{
				var meshes = (from i in content.objects where GH_Convert.ToMesh(i, ref m, GH_Conversion.Primary) select m).ToList();
				@base.objects = meshes.Select(o => defaultConverter.ConvertToSpeckle(o)).Where(i => i != null).ToList();
			}
			return @base;
		}
	}
}