using System.Collections.Generic;

namespace ViewObjects.Unity
{

	public class TargetContentMono : ContentMono, ITargetContent
	{
		public bool isolate { get; set; }
		public List<IViewerBundle> bundles { get; set; }
	}
}