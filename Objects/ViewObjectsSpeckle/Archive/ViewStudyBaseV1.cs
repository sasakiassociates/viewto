using System.Collections.Generic;

namespace ViewObjects.Speckle
{

	/// <summary>
	/// A View Study for Speckle  
	/// </summary>
	public class ViewStudyBaseV1 : ViewObjectBase_v1
	{

		/// <summary>
		/// 
		/// </summary>
		public ViewStudyBaseV1()
		{ }

		public string ViewName { get; set; }

		public List<ViewObjectBase_v1> objs { get; set; }
	}
}