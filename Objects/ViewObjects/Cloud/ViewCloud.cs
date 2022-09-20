using System;
using System.Collections.Generic;
using System.Linq;

namespace ViewObjects.Cloud
{
	public class ViewCloudReference : IViewObj, IViewCloud_v2
	{
		public string ViewId { get; set; }

		public List<string> References { get; set; }

		public ViewCloudReference()
		{
			this.ViewId = ObjUtils.InitGuid;
			this.References = new List<string>();
		}

		public ViewCloudReference(List<string> references, string viewId = null)
		{
			this.References = references;
			this.ViewId = ObjUtils.CheckIfValidId(viewId);
		}
	}

	public class ViewCloud : IViewCloud, IValidate
	{

		public ViewCloud() => ViewId = Guid.NewGuid().ToString();

		public virtual bool isValid
		{
			get => points != null && points.Any();
		}

		public CloudPoint[] points { get; set; }

		/// <summary>
		///   Temporary get function that returns a GUID that is set when object is created
		/// </summary>
		public string ViewId { get; set; }

		public int count
		{
			get => points.Valid() ? points.Length : 0;
		}
	}
}