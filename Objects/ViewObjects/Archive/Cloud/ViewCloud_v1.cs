using System;
using System.Linq;

namespace ViewObjects.Cloud
{

	public class ViewCloudV1V1 : IViewCloud_v1, IValidate
	{

		public ViewCloudV1V1() => ViewId = Guid.NewGuid().ToString();

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