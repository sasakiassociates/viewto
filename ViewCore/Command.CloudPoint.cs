using System.Collections.Generic;
using ViewObjects;

namespace ViewTo
{
	public static partial class Commander
	{
		public static IEnumerable<string> BasicParam()
		{
			return new[]
			{
				"X", "Y", "Z", "Xn", "Yn", "Zn"
			};
		}

		public static IEnumerable<double> Stack(CloudPoint po)
		{
			return new[]
			{
				po.x, po.y, po.z, po.xn, po.yn, po.zn
			};
		}
	}
}