using System.Collections.Generic;

namespace ViewObjects
{
	public sealed class ContentBundleV1 : IViewContentBundle_v1
	{
		public ContentBundleV1() => contents = new List<IViewContent_v1>();

		public List<IViewContent_v1> contents { get; set; }
	}
}