#region

using System.Collections.Generic;
using UnityEngine;
using ViewObjects;
using ViewTo.Events.Report;

#endregion

namespace ViewTo.Connector.Unity.Commands
{
	public static class Reporter
	{
		public static void Report(
			this IViewStudy obj,
			int targetCount, int isoTargetCount, int blockersCount, int designsCount,
			int globalBundleCount, int isoBundleCount,
			int cloudCount, int pointCount
		)
		{
			var args = new StudyReportArgs(
				obj.viewName, obj.CanRun(), obj.CanVisualize(),
				blockersCount, designsCount, targetCount,
				isoTargetCount, globalBundleCount, isoBundleCount, cloudCount, pointCount);

			Debug.Log(args.message);
		}

		public static int GetSum(this Dictionary<string, CloudPoint[]> items)
		{
			var res = 0;

			if (!items.Valid())
				return res;

			foreach (var points in items.Values)
				if (points.Valid())
					res += points.Length;

			return res;
		}
	}
}