using System;
using System.Collections.Generic;
using System.Linq;
using ViewObjects.Systems;
using ViewObjects.Unity;
using ViewTo.Connector.Unity.Commands;
using VO = ViewObjects;
using ViewObjects.Systems.Layouts;

namespace ViewTo.Connector.Unity
{

	[Serializable]
	public class ViewerSetupData
	{

		public List<ILayout> Layouts { get; private set; }

		public List<Content> ProposedContent { get; private set; }

		public List<ViewCloud> Clouds { get; private set; }

		public List<ViewColorWithName> Colors { get; private set; }

		public ViewerSetupData(RigParameters data)
		{
			Layouts = data.Viewer;
			Clouds = ViewObject.GetCloudsByKey(data.Clouds);
			ProposedContent = ViewObject.TryFetchInScene(VO.ViewContentType.Proposed);
			Colors = data.Colors.GetViewColorsFromScene().ToList();
		}

	}
}