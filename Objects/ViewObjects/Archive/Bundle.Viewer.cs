using System;
using System.Collections.Generic;
using ViewObjects.Cloud;

namespace ViewObjects.Viewer
{

	[Serializable]
	public class ViewerBundleV1 : IViewerBundle_v1
	{

		public virtual bool isValid
		{
			get => layouts.Valid();
		}

		public List<IViewerLayout_v1> layouts { get; set; }
	}

	public class ViewerBundleLinkedV1 : IViewerBundleLinked_v1
	{
		public List<CloudShell> linkedClouds { get; set; }

		public List<IViewerLayout_v1> layouts { get; set; }
	}
}