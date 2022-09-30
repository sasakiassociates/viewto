using System.Collections.Generic;
using Speckle.Core.Kits;
using Speckle.Core.Models;

namespace ViewObjects.Speckle
{
	/// <summary>
	///   View Content Object adapted to <see cref="Base" />
	/// </summary>
	public class Content : ViewObjectReferenceBase<ViewObjects.Content>, IContent
	{

		/// <inheritdoc />
		public Content()
		{ }

		/// <summary>
		///   Schema for constructing an object in speckle
		/// </summary>
		/// <param name="type">The type of view content to use</param>
		/// <param name="references">List commit ids to reference to use when building the content group</param>
		/// <param name="viewName">Optional name for the content</param>
		/// <param name="viewId">  Id of the view study as a <see cref="System.Guid" />. If no valid value is passed in one will be
		///   generated
		/// </param>
		[SchemaInfo("View Content", "Simple Object type for structuring geometry for a view study", ViewObjectSpeckle.Schema.Category, "Objects")]
		public Content(ContentType type, List<string> references, string viewId = null, string viewName = null)
		{
			ContentType = type;
			References = references;
			ViewName = viewName;
			ViewId = ObjUtils.CheckIfValidId(viewId);
		}

		/// <inheritdoc />
		public ContentType ContentType { get; set; }

	}
}