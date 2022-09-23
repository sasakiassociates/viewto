using System.Collections.Generic;
using Speckle.Core.Api;
using Speckle.Core.Kits;

namespace ViewObjects.Speckle
{
	/// <summary>
	/// A simple view study as a speckle object
	/// </summary>
	public class ViewStudyBase_v2 : ViewObjectBase_v2, IViewStudy_v2<ViewObjectBase_v2>
	{

		/// <summary>
		/// 
		/// </summary>
		public string ViewName { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string ViewId { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public List<ViewObjectBase_v2> Objects { get; set; }

		/// <summary>
		/// Default constructor for converting
		/// </summary>
		public ViewStudyBase_v2()
		{
			Objects = new List<ViewObjectBase_v2>();
		}

		/// <summary>
		/// Schema constructor for creating a view study
		/// </summary>
		/// <param name="objects">Objects to use for view studies</param>
		/// <param name="viewName">Name of the study. Helpful to have this related to the branch <see cref="Branch.name"/></param>
		/// <param name="viewId">Id of the view study as a <see cref="System.Guid"/>. If no valid value is passed in one will be generated</param>
		[SchemaInfo("View Study", "View Study Object for setting up a study in View To", ViewObjectSpeckle.Schema.Category, "Objects")]
		public ViewStudyBase_v2(List<ViewObjectBase_v2> objects, string viewName, string viewId = null)
		{
			this.Objects = objects;
			this.ViewName = viewName;
			this.ViewId = viewId.CheckIfValidId();
		}

	}
}