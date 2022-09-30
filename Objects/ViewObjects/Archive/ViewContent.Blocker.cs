using System.Collections.Generic;

namespace ViewObjects
{
	public class BlockerContentV1 : IBlockerContentV1
	{
		public string ViewName { get; set; } = "Blocker";

		public ViewColor viewColor { get; set; }

		public List<object> objects { get; set; }
	}
}