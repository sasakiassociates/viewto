using System.Collections.Generic;

namespace ViewObjects.Content
{
	public sealed class ContentBundle : IViewContentBundle
	{
		public ContentBundle() => contents = new List<IViewContent>();

		public List<IViewContent> contents { get; set; }
	}
}