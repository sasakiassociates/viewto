using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace ViewObjects.Unity
{
	public static partial class ViewObject
	{

		public static List<ViewCloud> GetCloudsByKey(List<string> ids)
		{
			var viewClouds = new List<ViewCloud>();

			if (!ids.Valid())
			{
				return null;
			}

			foreach (var key in ids)
			{
				var obj = TryFetchInScene<ViewCloud>(key);
				if (obj != null)
					viewClouds.Add(obj);
			}

			return viewClouds;
		}

		public static ViewColor ToView(this Color32 value) => new(value.r, value.g, value.b, value.a);

		public static List<Color32> ToUnity(this IEnumerable<ViewColor> value)
		{
			var res = new List<Color32>();
			foreach (var v in value)
				res.Add(v.ToUnity());

			return res;
		}

		public static Color32 ToUnity(this ViewColor value)
		{
			return value != null ? new Color32(value.R, value.G, value.B, value.A) : default;
		}
	}
}