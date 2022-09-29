using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ViewObjects.Content;
using ViewObjects.Explorer;

namespace ViewObjects
{
	public static class ObjUtils
	{

		/// <summary>
		/// Shorthand for getting trimming the object type name to the last value
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string TypeName(this object obj) => obj != null ? obj.GetType().ToString().Split('.').Last() : "null";

		/// <summary>
		/// Shorthand for checking list object
		/// </summary>
		/// <param name="list"></param>
		/// <returns>returns true if list contains at least one item</returns>
		public static bool Valid(this IList list) => list != null && list.Count > 0;

		/// <summary>
		/// Shorthand for checking list object
		/// Use <seealso cref="Valid(System.Collections.IList)"/> when using <paramref name="count"/> as 0
		/// </summary>
		/// <param name="list"></param>
		/// <param name="count">Any value above 0</param>
		/// <returns>if list is not null and has the min <paramref name="count"/> of items </returns>
		public static bool Valid(this IList list, int count) => list != null && list.Count > count;

		/// <summary>
		/// Shorthand for checking list object
		/// </summary>
		/// <param name="list"></param>
		/// <returns>returns true if list contains at least one item</returns>
		public static bool Valid(this ICollection list) => list != null && list.Count > 0;

		/// <summary>
		/// Shorthand for checking list object
		/// Use <seealso cref="Valid(System.Collections.ICollection)"/> when using <paramref name="count"/> as 0
		/// </summary>
		/// <param name="list"></param>
		/// <param name="count">Any value above 0</param>
		/// <returns>if list is not null and has the min <paramref name="count"/> of items </returns>
		public static bool Valid(this ICollection list, int count) => list != null && list.Count > count;

		/// <summary>
		/// Shorthand for checking if string is null or empty
		/// </summary>
		/// <param name="value"></param>
		/// <returns>returns true if value contains something</returns>
		public static bool Valid(this string value) => !string.IsNullOrEmpty(value);

		public static string CheckIfValidId(string valueId) =>
			!string.IsNullOrEmpty(valueId) && Guid.TryParse(valueId, out _) ? valueId : Guid.NewGuid().ToString();

		public static string InitGuid => Guid.NewGuid().ToString();

		public static TObj FindObject<TObj>(this IViewStudy_v2 obj) where TObj : IViewObj
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

		public static List<TObj> FindObjects<TObj>(this IViewStudy_v2 obj) where TObj : IViewObj
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

		public static List<ContentOption> GetContentOptions(this IViewStudy_v2 obj)
		{
			List<ContentOption> viewObjs = new();

			var contents = obj.FindObjects<ViewContent_v2>();
			if (contents.Any())
			{
				foreach (var o in contents)
				{
					// viewObjs.Add(new ContentOption(){stage = });
					// if (o is TObj casted)
					// {
					// 	viewObjs.Add(casted);
					// }
				}
			}

			return viewObjs;
		}

	}
}