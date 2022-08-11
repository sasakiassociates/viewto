using System.Collections.Generic;

namespace ViewObjects.Speckle
{

	/// <summary>
	/// A View Study for Speckle  
	/// </summary>
	public class ViewStudyBase : ViewObjBase
	{

		/// <summary>
		/// 
		/// </summary>
		public ViewStudyBase()
		{ }

		public string viewName { get; set; }

		public List<ViewObjBase> objs { get; set; }
	}
}