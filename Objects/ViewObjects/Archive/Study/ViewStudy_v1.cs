using System.Collections.Generic;

namespace ViewObjects.Study
{
	public class ViewStudy_v1 : IViewStudy_v1
	{
		public ViewStudy_v1() => objs = new List<IViewObject>();

		public string ViewName { get; set; }

		public bool IsValid
		{
			get => ViewName.Valid() && objs.Valid();
		}

		public List<IViewObject> objs { get; set; }
	}

}