using System.Collections.Generic;

namespace ViewObjects.Study
{
	public class ViewStudy : IViewStudy
	{
		public ViewStudy() => objs = new List<IViewObj>();

		public string viewName { get; set; }
		public bool isValid
		{
			get => viewName.Valid() && objs.Valid();
		}
		public List<IViewObj> objs { get; set; }
	}
}