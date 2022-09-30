using System.Collections.Generic;

namespace ViewObjects
{
	public class DesignContentV1 : IDesignContentV1
	{
		public string ViewName { get; set; }

		public ViewColor viewColor { get; set; }

		public List<object> objects { get; set; }
	}
}