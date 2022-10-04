using System.Collections.Generic;
using ViewTo.RhinoGh.Goo;

namespace ViewTo.RhinoGh
{
	public static class ViewToHelper
	{

		public static List<TBase> Unwrap<TBase>(this List<GH_ViewObj> wrappers)
		{
			var items = new List<TBase>();
			foreach (var w in wrappers)
			{
				if (w.Value is TBase @base)
				{
					items.Add(@base);
				}
			}

			return items;
		}
	}
}