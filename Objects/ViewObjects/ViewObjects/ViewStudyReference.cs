using System.Collections.Generic;
using ViewObjects.References;

namespace ViewObjects.Study
{
	public class ViewStudyReference : IViewStudy<ViewObjectReference>, IViewObject
	{
		/// <inheritdoc />
		public string ViewId { get; }

		/// <inheritdoc />
		public string ViewName { get; set; }

		/// <inheritdoc />
		public List<ViewObjectReference> Objects { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public ViewStudyReference()
		{
			ViewId = ObjUtils.InitGuid;
			Objects = new List<ViewObjectReference>();
		}

		public ViewStudyReference(List<ViewObjectReference> objects, string viewName, string viewId = null)
		{
			ViewName = viewName;
			ViewId = ObjUtils.CheckIfValidId(viewId);
			Objects = objects ?? new List<ViewObjectReference>();
		}
	}
}