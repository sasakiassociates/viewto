#region

using System.Collections.Generic;
using UnityEngine;
using ViewObjects;
using ViewObjects.Unity;

#endregion

namespace ViewTo.Connector.Unity
{
	public class RigParamData : ScriptableObject, IRigParam
	{
		public bool isolate;
		public List<ViewColor> contentColors;
		public List<ViewCloudMono> clouds;
		public List<SoViewerBundle> viewers;

		public RigParamData()
		{
			contentColors = new List<ViewColor>();
			clouds = new List<ViewCloudMono>();
			viewers = new List<SoViewerBundle>();
			bundles = new List<IViewerBundle>();
		}

		public List<IViewerBundle> bundles { get; set; }
	}
}