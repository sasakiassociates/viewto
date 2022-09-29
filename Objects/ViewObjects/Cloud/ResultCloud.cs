using System;
using System.Collections.Generic;
using System.Linq;

namespace ViewObjects.Cloud
{
	public class ResultCloud : ViewCloud, IResultCloud
	{
		public ResultCloud() => ViewId = Guid.NewGuid().ToString();

		public override bool IsValid
		{
			get => base.IsValid && data != null && data.Any();
		}

		public List<IResultData> data { get; set; }
	}

	public class ResultCloud_v2 : IResultCloud_v2, IViewObj
	{

		public ResultCloud_v2() => ViewId = ObjUtils.InitGuid;

		public ResultCloud_v2(CloudPoint[] points, List<IResultCloudData> data, string viewId = null)
		{
			Points = points;
			Data = data;
			ViewId = ObjUtils.CheckIfValidId(viewId);
		}

		/// <inheritdoc />
		public string ViewId { get; }

		/// <inheritdoc />
		public CloudPoint[] Points { get; set; } = Array.Empty<CloudPoint>();

		/// <inheritdoc />
		public List<IResultCloudData> Data { get; set; } = new();
	}
}