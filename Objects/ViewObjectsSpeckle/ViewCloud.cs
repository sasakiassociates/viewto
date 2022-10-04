using System.Collections.Generic;
using Speckle.Core.Kits;

namespace ViewObjects.Speckle
{
	/// <summary>
	/// </summary>
	public class ViewCloud : ViewObjectReferenceBase<Cloud.ViewCloud>
	{

		/// <summary>
		/// </summary>
		public ViewCloud()
		{ }

		/// <summary>
		///   Schema for constructing an object in speckle
		/// </summary>
		/// <param name="references">List commit ids to reference to use when building the content group</param>
		/// <param name="viewId">
		///  Id of the view study as a <see cref="System.Guid" />. If no valid value is passed in one will be
		///   generated
		/// </param>
		[SchemaInfo("View Content", "Simple Object type for structuring geometry for a view study", ViewObject.Schema.Category, "Objects")]
		public ViewCloud(List<string> references, string viewId = null)
		{
			References = references;
			ViewId = ObjUtils.CheckIfValidId(viewId);
		}

	}
}