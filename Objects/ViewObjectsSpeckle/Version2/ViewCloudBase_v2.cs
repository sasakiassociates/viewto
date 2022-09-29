using System.Collections.Generic;
using Speckle.Core.Kits;

namespace ViewObjects.Speckle
{
	/// <summary>
	/// 
	/// </summary>
	public class ViewCloudShellBaseV2 : ViewObjectBase_v2, IViewCloudRef_v2
	{

		/// <inheritdoc />
		public string ViewId { get; set; }

		/// <inheritdoc />
		public List<string> References { get; set; } = new List<string>();

		/// <summary>
		/// 
		/// </summary>
		public ViewCloudShellBaseV2()
		{ }

		/// <summary>
		/// Schema for constructing an object in speckle
		/// </summary>
		/// <param name="references">List commit ids to reference to use when building the content group</param>
		/// <param name="viewId">Id of the view study as a <see cref="System.Guid"/>. If no valid value is passed in one will be generated</param>
		[SchemaInfo("View Content", "Simple Object type for structuring geometry for a view study", ViewObjectSpeckle.Schema.Category, "Objects")]
		public ViewCloudShellBaseV2(List<string> references, string viewId = null)
		{
			this.References = references;
			this.ViewId = ObjUtils.CheckIfValidId(viewId);
		}

	}
}