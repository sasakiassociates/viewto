using System.Collections.Generic;
using Speckle.Core.Api;
using Speckle.Core.Kits;

namespace ViewObjects.Speckle
{
	/// <summary>
	/// 
	/// </summary>
	public class ViewCloudBase_v2 : ViewObjectBase_v2, IViewCloud_v2
	{

		/// <inheritdoc />
		public string ViewId { get; set; }

		/// <summary>
		/// List of Commit <see cref="Commit.id"/> to reference in a <see cref="IViewStudy_v2{TObject}"/>
		/// </summary>
		public List<string> References { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public ViewCloudBase_v2()
		{ }

		/// <summary>
		/// Schema for constructing an object in speckle
		/// </summary>
		/// <param name="references">List commit ids to reference to use when building the content group</param>
		/// <param name="viewId">Id of the view study as a <see cref="System.Guid"/>. If no valid value is passed in one will be generated</param>
		[SchemaInfo("View Content", "Simple Object type for structuring geometry for a view study", ViewObjectSpeckle.Schema.Category, "Objects")]
		public ViewCloudBase_v2(List<string> references, string viewId = null)
		{
			this.References = references;
			this.ViewId = ObjUtils.CheckIfValidId(viewId);
		}

	}
}