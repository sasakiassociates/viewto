using System.Collections.Generic;
using System.Linq;

namespace ViewObjects
{
	public static class Extensions
	{
		public static List<IContentOption> FindObject(this IResultCloud<IResultCloudData> obj) => new();

		public static TObj FindObject<TObj>(this IViewStudy obj) where TObj : IViewObject
		{
			TObj viewObj = default;
			if (!obj.Objects.Any())
			{
				return viewObj;
			}

			foreach (var o in obj.Objects)
			{
				if (o is TObj casted)
				{
					viewObj = casted;
					break;
				}
			}

			return viewObj;
		}

		public static List<TObj> FindObjects<TObj>(this IViewStudy obj) where TObj : IViewObject
		{
			List<TObj> viewObjs = new();

			if (obj.Objects.Any())
			{
				foreach (var o in obj.Objects)
				{
					if (o is TObj casted)
					{
						viewObjs.Add(casted);
					}
				}
			}

			return viewObjs;
		}
	}

}