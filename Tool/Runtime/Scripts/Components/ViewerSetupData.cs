using System;
using System.Collections.Generic;
using System.Linq;
using ViewObjects.Unity;
using ViewTo.Connector.Unity.Commands;
using VO = ViewObjects;

namespace ViewTo.Connector.Unity
{

	[Serializable]
	public class ViewerSetupData
	{

		public List<VO.IViewerLayout> Layouts { get; private set; }

		public List<Content> ProposedContent { get; private set; }

		public List<ViewCloud> Clouds { get; private set; }

		public List<ViewColorWithName> Colors { get; private set; }

		public ViewerSetupData(VO.RigParameters data)
		{
			Layouts = data.Viewer;
			Clouds = ViewObject.GetCloudsByKey(data.Clouds);
			ProposedContent = ViewObject.TryFetchInScene(VO.ContentType.Proposed);
			Colors = data.Colors.GetViewColorsFromScene().ToList();
		}

	}
}