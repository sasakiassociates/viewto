using System.Collections.Generic;
using ViewObjects;
using ViewObjects.Cloud;
using ViewTo.RhinoGh.Goo;

namespace ViewTo.RhinoGh
{
	public static class ViewToHelper
	{

		public static CloudShell Build(this ViewCloudV1V1 obj) => new CloudShell(obj, obj.ViewId, obj.points.Valid() ? obj.points.Length : 0);

		public static List<TBase> Unwrap<TBase>(this List<GH_ViewObj> wrappers)
		{
			var items = new List<TBase>();
			foreach (var w in wrappers)
				if (w.Value is TBase @base)
					items.Add(@base);

			return items;
		}
	}
}