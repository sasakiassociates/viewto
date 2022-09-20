using System.Collections.Generic;

namespace ViewObjects.Content
{
	public class DesignContent : IDesignContent
	{
		public string ViewName { get; set; }

		public ViewColor viewColor { get; set; }

		public List<object> objects { get; set; }
	}
}