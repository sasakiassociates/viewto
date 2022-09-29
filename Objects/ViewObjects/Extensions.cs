using System.Collections.Generic;
using System.Linq;

namespace ViewObjects
{
	public static class Extensions
	{
		public static List<IContentOption> FindObject(this IResultCloud_v2<IResultCloudData> obj) => new List<IContentOption>();

		public static TObj FindObject<TObj>(this IViewStudy_v2 obj) where TObj : IViewObj
		{
			TObj viewObj = default;
			if (!obj.Objects.Any()) return viewObj;

			foreach (var o in obj.Objects)
				if (o is TObj casted)
				{
					viewObj = casted;
					break;
				}

			return viewObj;
		}

		public static List<TObj> FindObjects<TObj>(this IViewStudy_v2 obj) where TObj : IViewObj
		{
			List<TObj> viewObjs = new();

			if (obj.Objects.Any())
				foreach (var o in obj.Objects)
					if (o is TObj casted)
						viewObjs.Add(casted);

			return viewObjs;
		}
	}
}