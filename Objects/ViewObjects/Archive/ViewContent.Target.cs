using System.Collections.Generic;

namespace ViewObjects
{

	public class TargetContentV1 : ITargetContentV1
	{
		public string ViewName { get; set; }

		public ViewColor viewColor { get; set; }

		public List<object> objects { get; set; }

		public bool isolate { get; set; }

		public List<IViewerBundle_v1> bundles { get; set; }
	}

}