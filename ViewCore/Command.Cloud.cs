using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ViewObjects;
using ViewObjects.Cloud;
using ViewTo.Commands;

namespace ViewTo
{
	public static partial class Commander
	{
		public static string WriteToCsv(this IResultCloudV1 obj, string location)
		{
			var file = new CloudToCsvCommand(obj);
			file.Run();

			if (!file.args.success)
			{
				Console.WriteLine("Cloud was not compiled correctly");
				return"";
			}

			var path = Path.Combine(location, obj.ViewId + ".csv");
			new WriteCsvCommand(file.args.data, path).Run();

			return path;
		}

		public static CloudShell Build(this IViewCloud_v1 obj) => new CloudShell(obj, obj.ViewId, obj.points.Valid() ? obj.points.Length : 0);

		internal static IEnumerable<string> GetIds(this IEnumerable<IViewCloud_v1> objs)
		{
			return objs.Select(o => o.ViewId).ToList();
		}
	}
}