using System;
using System.Linq;

namespace ViewObjects.Cloud
{
	public class ViewCloud : IViewCloud, IValidate
	{

		public ViewCloud() => viewID = Guid.NewGuid().ToString();

		public virtual bool isValid
		{
			get => points != null && points.Any();
		}

		public CloudPoint[] points { get; set; }
		/// <summary>
		///   Temporary get function that returns a GUID that is set when object is created
		/// </summary>
		public string viewID { get; set; }

		public int count
		{
			get => points.Valid() ? points.Length : 0;
		}
	}
}