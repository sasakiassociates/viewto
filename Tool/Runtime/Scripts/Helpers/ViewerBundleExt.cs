#region

using System.Collections.Generic;
using ViewObjects;
using ViewObjects.Viewer;

#endregion

namespace ViewTo.Connector.Unity
{
	public static class ViewerBundleExt
	{
		public static void Set(this IRigParam param, ViewerLayout layout)
		{
			if (param == null)
			{
				ViewConsole.Log("Trying to add new layout to rig but paras are null");
				return;
			}

			if (!param.bundles.Valid())
				param.bundles = new List<IViewerBundle>();

			param.bundles.Add(new ViewerBundle
			{
				layouts = new List<IViewerLayout>
					{ layout }
			});
		}
	}
}