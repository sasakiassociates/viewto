using System.Collections.Generic;
using Speckle.Core.Api;
using Speckle.Core.Kits;

namespace ViewObjects.Speckle
{
	/// <summary>
	///   A simple view study as a speckle object
	/// </summary>
	public class ViewStudy : ViewObjectBase, IViewStudy<ViewObjectBase>
	{

		/// <summary>
		///   Default constructor for converting
		/// </summary>
		public ViewStudy()
		{ }

		/// <summary>
		///   Schema constructor for creating a view study
		/// </summary>
		/// <param name="objects">Objects to use for view studies</param>
		/// <param name="viewName">Name of the study. Helpful to have this related to the branch <see cref="Branch.name" /></param>
		/// <param name="viewId">
		///   Id of the view study as a <see cref="System.Guid" />. If no valid value is passed in one will be
		///   generated
		/// </param>
		[SchemaInfo("View Study", "View Study Object for setting up a study in View To", ViewObjectSpeckle.Schema.Category, "Objects")]
		public ViewStudy(List<ViewObjectBase> objects, string viewName, string viewId = null)
		{
			Objects = objects;
			ViewName = viewName;
			ViewId = viewId.CheckIfValidId();
		}

		/// <inheritdoc />
		public string ViewName { get; set; }

		/// <inheritdoc />
		public string ViewId { get; set; }

		/// <inheritdoc />
		public List<ViewObjectBase> Objects { get; set; } = new List<ViewObjectBase>();
	}
}