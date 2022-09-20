using System;
using System.Collections.Generic;

namespace ViewObjects.Content
{
	public class ViewContent_v2 : IContent, IStreamReference, IViewObj
	{

		public string ViewName { get; set; }

		public string ViewId { get; set; }

		public List<string> References { get; set; }

		public ContentType ContentType { get; }

		/// <summary>
		/// 
		/// </summary>
		public ViewContent_v2()
		{
			References = new List<string>();
			ViewId = Guid.NewGuid().ToString();
		}

		/// <summary>
		/// Constructs a content for a view study 
		/// </summary>
		/// <param name="references">List of object ids to reference in speckle</param>
		/// <param name="type">The type of content to label this as</param>
		/// <param name="viewName">Name of the view study</param>
		/// <param name="viewId">Id of the view study as a <see cref="System.Guid"/>. If no valid value is passed in one will be generated</param>
		public ViewContent_v2(List<string> references, ContentType type, string viewId = null, string viewName = null)
		{
			this.ContentType = type;
			this.References = references;
			this.ViewName = viewName;
			this.ViewId = ObjUtils.CheckIfValidId(viewId);
		}

		/// <summary>
		/// Constructs a content for a view study 
		/// </summary>
		/// <param name="type">The type of content to label this as</param>
		/// <param name="viewName">Name of the view study</param>
		/// <param name="viewId">Id of the view study as a <see cref="System.Guid"/>. If no valid value is passed in one will be generated</param>
		public ViewContent_v2(ContentType type, string viewId = null, string viewName = null)
		{
			this.ContentType = type;
			this.References = new List<string>();
			this.ViewName = viewName;
			this.ViewId = ObjUtils.CheckIfValidId(viewId);
		}
	}
}