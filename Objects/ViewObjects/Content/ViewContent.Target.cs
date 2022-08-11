using System.Collections.Generic;

namespace ViewObjects.Content
{

	public class TargetContent : ITargetContent
	{
		public string viewName { get; set; }
		public ViewColor viewColor { get; set; }
		public List<object> objects { get; set; }

		public bool isolate { get; set; }
		public List<IViewerBundle> bundles { get; set; }
	}

}