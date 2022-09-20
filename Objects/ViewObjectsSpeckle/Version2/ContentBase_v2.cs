using System.Collections.Generic;
using Speckle.Core.Api;
using Speckle.Core.Kits;
using Speckle.Core.Models;

namespace ViewObjects.Speckle
{
	/// <summary>
	/// View Content Object adapted to <see cref="Base"/>
	/// </summary>
	public class ContentBase_v2 : ViewObjectBase, IContent
	{
		/// <inheritdoc />
		public string ViewName { get; set; }

		/// <inheritdoc />
		public string ViewId { get; set; }

		/// <inheritdoc />
		public ContentType ContentType { get; set; }

		/// <summary>
		/// List of Commit <see cref="Commit.id"/> to reference in a <see cref="IViewStudy_v2"/>
		/// </summary>
		public List<string> References { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public ContentBase_v2()
		{ }

		/// <summary>
		/// Schema for constructing an object in speckle
		/// </summary>
		/// <param name="type">The type of view content to use</param>
		/// <param name="references">List commit ids to reference to use when building the content group</param>
		/// <param name="viewName">Optional name for the content</param>
		/// <param name="viewId">Id of the view study as a <see cref="System.Guid"/>. If no valid value is passed in one will be generated</param>
		[SchemaInfo("View Content", "Simple Object type for structuring geometry for a view study", ViewObjectSpeckle.Schema.Category, "Objects")]
		public ContentBase_v2(ContentType type, List<string> references, string viewId = null, string viewName = null)
		{
			this.ContentType = type;
			this.References = references;
			this.ViewName = viewName;
			this.ViewId = ObjUtils.CheckIfValidId(viewId);
		}

	}
}