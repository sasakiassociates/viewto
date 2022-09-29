using System;
using System.Collections.Generic;

namespace ViewObjects.Study
{
	public class ViewStudy : IViewStudy
	{
		public ViewStudy() => objs = new List<IViewObj>();

		public string ViewName { get; set; }

		public bool IsValid
		{
			get => ViewName.Valid() && objs.Valid();
		}

		public List<IViewObj> objs { get; set; }
	}

	public class ViewStudy_v2 : IViewStudy_v2, IViewObj
	{
		/// <summary>
		/// Name of the view study
		/// </summary>
		public string ViewName { get; set; }

		/// <summary>
		/// Id for in the format as <see cref="System.Guid"/>
		/// </summary>
		public string ViewId { get; set; }

		/// <summary>
		/// Group of that make up the view study
		/// </summary>
		public List<IViewObj> Objects { get; set; }

		public ViewStudy_v2()
		{
			Objects = new List<IViewObj>();
			ViewId = Guid.NewGuid().ToString();
		}

		/// <summary>
		/// Constructs a view study
		/// </summary>
		/// <param name="objects">List of <see cref="IViewObj"/> to use</param>
		/// <param name="viewName">Name of the view study</param>
		/// <param name="viewId">Id of the view study as a <see cref="System.Guid"/>. If no valid value is passed in one will be generated</param>
		public ViewStudy_v2(List<IViewObj> objects, string viewName, string viewId = null)
		{
			this.Objects = objects;
			this.ViewName = viewName;
			this.ViewId = ObjUtils.CheckIfValidId(viewId);
		}
	}
}