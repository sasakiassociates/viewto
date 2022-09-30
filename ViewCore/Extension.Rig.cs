using System;
using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewObjects.Cloud;
using ViewTo.Events.Report;

namespace ViewTo
{
	public static partial class ViewCoreExtensions
	{

		public static void Load(this IRig_v1 obj, string cloudID, CloudPoint[] points)
		{
			Console.WriteLine(points.Length);
			obj.clouds ??= new Dictionary<string, CloudPoint[]>();
			obj.clouds.Add(cloudID, points);
		}

		public static void Load(this IRig_v1 obj, List<ViewColor> globalColors, IEnumerable<IRigParam_v1> bundles)
		{
			obj.globalColors = globalColors;
			obj.globalParams = bundles.ToList();
		}

		internal static void ReportSetup(this IRig_v1 obj, Action<RigSetupReportArgs> args)
		{
			var bundles = new List<IViewerBundle_v1>();

			foreach (var g in obj.globalParams)
				bundles.AddRange(g.bundles);

			var clouds = obj.clouds.Select(p => new CloudShell(typeof(ViewCloudV1V1), p.Key, p.Value?.Length ?? 0))
				.ToList();

			args?.Invoke(new RigSetupReportArgs(bundles, obj.globalColors, clouds));
		}
	}
}