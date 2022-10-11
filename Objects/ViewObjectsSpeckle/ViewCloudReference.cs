using System.Collections.Generic;
using Speckle.Core.Kits;

namespace ViewObjects.Speckle
{
	/// <summary>
	/// </summary>
	public class ViewCloudReference : ViewObjectReference<ViewObjects.ViewCloudReference>
	{

		/// <summary>
		/// 
		/// </summary>
		public ViewCloudReference()
		{ }

		/// <inheritdoc />
		[SchemaInfo("View Cloud", "Simple Object type for structuring geometry for a view study", ViewObject.Schema.Category, "Objects")]
		public ViewCloudReference(ViewObjects.ViewCloudReference obj, List<string> references) : base(obj, references)
		{ }

		/// <inheritdoc />
		[SchemaInfo("View Cloud", "Simple Object type for structuring geometry for a view study", ViewObject.Schema.Category, "Objects")]
		public ViewCloudReference(ViewObjects.ViewCloudReference obj) : base(obj)
		{
			References = obj.References;
		}

		/// <inheritdoc />
		public ViewCloudReference(List<string> references, string viewId) : base(references, viewId, null)
		{ }
	}
}