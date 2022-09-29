using System;
using System.Collections.Generic;
using System.Linq;

namespace ViewObjects.Cloud
{
	public class ViewCloud_v2 : IViewCloud_v2, IViewObj
	{

		/// <inheritdoc />
		public string ViewId { get; set; }

		/// <inheritdoc />
		public CloudPoint[] Points { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public ViewCloud_v2()
		{
			ViewId = ObjUtils.InitGuid;
		}
	}

	public class ViewCloudReference : IViewObj, IViewCloudRef_v2
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

		public virtual bool IsValid
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