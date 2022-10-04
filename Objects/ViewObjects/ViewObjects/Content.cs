using System;
using System.Collections.Generic;

namespace ViewObjects
{
	public class Content : IViewObject, IContent, IContentObjects<object>
	{

		/// <summary>
		/// </summary>
		public Content() => ViewId = Guid.NewGuid().ToString();

		/// <summary>
		///   Constructs a content for a view study
		/// </summary>
		/// <param name="type">The type of content to label this as</param>
		/// <param name="viewName">Name of the view study</param>
		/// <param name="viewId">
		///   Id of the view study as a <see cref="System.Guid" />. If no valid value is passed in one will be
		///   generated
		/// </param>
		public Content(ContentType type, string viewId = null, string viewName = null)
		{
			ContentType = type;
			ViewName = viewName;
			ViewId = ObjUtils.CheckIfValidId(viewId);
		}

		/// <inheritdoc />
		public string ViewId { get; }

		/// <inheritdoc />
		public string ViewName { get; set; }

		/// <inheritdoc />
		public ContentType ContentType { get; }

		/// <inheritdoc />
		public ViewColor Color { get; set; }

		/// <inheritdoc />
		public List<object> Objects { get; set; }
	}
}