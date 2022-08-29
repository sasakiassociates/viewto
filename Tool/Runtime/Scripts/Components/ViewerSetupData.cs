#region

using System.Collections.Generic;
using ViewObjects;
using ViewObjects.Unity;

#endregion

namespace ViewTo.Connector.Unity
{

	public readonly struct ViewerSetupData
	{
		public ViewerSetupData(
			List<ViewCloudMono> clouds,
			List<IViewerLayout> layouts,
			List<ViewColorWithName> viewColors,
			List<DesignContentMono> designContent
		)
		{
			this.clouds = clouds;
			this.layouts = layouts;
			this.viewColors = viewColors;
			this.designContent = designContent;
		}

		public readonly List<ViewCloudMono> clouds;
		public readonly List<ViewColorWithName> viewColors;
		public readonly List<DesignContentMono> designContent;
		public readonly List<IViewerLayout> layouts;

	}

}