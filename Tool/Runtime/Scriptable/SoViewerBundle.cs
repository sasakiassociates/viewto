#region

using System.Collections.Generic;
using UnityEngine;
using ViewObjects;
using ViewObjects.Unity;

#endregion

namespace ViewTo.Connector.Unity
{
	public class SoViewerBundle : ScriptableObject
	{
		// public List<SoViewerLayout> items;
		// public List<ViewCloudMono> linkedCloud;
		public List<ViewerLayoutMono> layouts;

		public void Ref(IViewerBundle bundle)
		{
			// items = new List<SoViewerLayout>();
			layouts = new List<ViewerLayoutMono>();

			if (bundle.layouts.Valid())
				foreach (var layout in bundle.layouts)
					if (layout is ViewerLayoutMono mono)
						layouts.Add(mono);
			// var so = CreateInstance<SoViewerLayout>();
			// //TODO Remove need for this ref
			// so.SetRef(layout);
			// items.Add(so);
		}
	}
}