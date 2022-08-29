#region

using System.Collections.Generic;
using ViewObjects;
using ViewObjects.Unity;
using ViewTo.Events.Report;

#endregion

namespace ViewTo.Connector.Unity.Commands
{
	public interface ICommandArgs
	{ }

	public class GetRigSystemArgs : ICommandArgs
	{

		public readonly List<ViewColorWithName> activeColorsInScene;
		public readonly List<DesignContentMono> designs;
		public readonly List<ViewColor> globalColors;
		public readonly List<RigParamData> rigParams;
		public readonly List<ViewerBundleSystem> viewers;

		public GetRigSystemArgs(PrimedRigArgs args, List<DesignContentMono> designs) => this.designs = designs;

		static List<ViewCloudMono> GetCloudsByKey(List<string> ids)
		{
			var viewClouds = new List<ViewCloudMono>();

			if (!ids.Valid())
			{
				ViewConsole.Warn("No valid clouds available to use for global viewer");
				return null;
			}

			foreach (var key in ids)
			{
				var obj = ViewObjMonoExt.TryFetchInScene<ViewCloudMono>(key);
				if (obj != null)
					viewClouds.Add(obj);
			}

			return viewClouds;
		}

		static List<IViewerLayout> GetGlobalLayouts(List<IRigParam> @params)
		{
			var globalLayouts = new List<IViewerLayout>();

			if (!@params.Valid())
			{
				ViewConsole.Warn("No active Global Params-Skipping Build");
				return globalLayouts;
			}

			foreach (var rp in @params)
			{
				if (rp == null) continue;

				if (!rp.bundles.Valid())
				{
					ViewConsole.Warn($"{rp.GetType()} does not have any bundles");
					continue;
				}

				foreach (var b in rp.bundles)
					if (b.layouts.Valid())
						globalLayouts.AddRange(b.layouts);
			}

			return globalLayouts;
		}
	}
}