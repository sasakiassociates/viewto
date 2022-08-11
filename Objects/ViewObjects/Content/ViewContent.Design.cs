using System.Collections.Generic;

namespace ViewObjects.Content
{
	public class DesignContent : IDesignContent
	{
		public string viewName { get; set; }
		public ViewColor viewColor { get; set; }
		public List<object> objects { get; set; }
	}
}