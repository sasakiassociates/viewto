using System.Collections.Generic;

namespace ViewObjects.Content
{
	public class BlockerContent : IBlockerContent
	{
		public string viewName { get; set; } = "Blocker";
		public ViewColor viewColor { get; set; }
		public List<object> objects { get; set; }
	}
}