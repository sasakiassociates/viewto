using System;
using System.Collections.Generic;
using ViewObjects.Cloud;

namespace ViewObjects.Viewer
{
	public class ViewerSystem_v2 : IViewerSystem_v2<IViewerLayout_v2>, IViewObj
	{

		public List<IViewerLayout_v2> Layouts { get; set; }

		public List<string> Clouds { get; set; }

		public ViewerSystem_v2()
		{
			this.Layouts = new List<IViewerLayout_v2>();
			this.Clouds = new List<string>();
		}

		public ViewerSystem_v2(List<IViewerLayout_v2> layouts, List<string> clouds = null)
		{
			this.Layouts = layouts;
			this.Clouds = clouds.Valid() ? clouds : new List<string>();
		}
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