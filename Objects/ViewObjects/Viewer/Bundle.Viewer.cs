using System;
using System.Collections.Generic;
using ViewObjects.Cloud;

namespace ViewObjects.Viewer
{
	public class ViewerSystem_v2 : IViewerSystem_v2<IViewerLayout_v2>, IViewObj
	{

		public ViewerSystem_v2()
		{
			Layouts = new List<IViewerLayout_v2>();
			Clouds = new List<string>();
		}

		public ViewerSystem_v2(List<IViewerLayout_v2> layouts, List<string> clouds = null)
		{
			Layouts = layouts;
			Clouds = clouds.Valid() ? clouds : new List<string>();
		}

		public List<IViewerLayout_v2> Layouts { get; set; }

		public List<string> Clouds { get; set; }
	}

	[Serializable]
	public class ViewerBundle : IViewerBundle
	{

		public virtual bool isValid
		{
			get => layouts.Valid();
		}

		public List<IViewerLayout> layouts { get; set; }
	}

	public class ViewerBundleLinked : IViewerBundleLinked
	{
		public List<CloudShell> linkedClouds { get; set; }

		public List<IViewerLayout> layouts { get; set; }
	}
}