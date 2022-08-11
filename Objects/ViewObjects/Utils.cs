using System.Collections;
using System.Linq;

namespace ViewObjects
{
	public static class ObjUtils
	{

		public static string TypeName(this object obj) => obj != null ? obj.GetType().ToString().Split('.').Last() : "null";

		public static bool Valid(this IList list) => list.Valid(0);

		public static bool Valid(this IList list, int count) => list != null && list.Count > count;

		public static bool Valid(this ICollection list) => list.Valid(0);

		public static bool Valid(this ICollection list, int count) => list != null && list.Count > count;

		public static bool Valid(this string value) => !string.IsNullOrEmpty(value);
	}
}