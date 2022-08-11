using System;
using System.Collections.Generic;
using System.Linq;
using Objects.Geometry;
using Speckle.Core.Models;
using Speckle.Newtonsoft.Json;

namespace ViewObjects.Speckle
{

	public class ViewCloudBase : ViewObjBase, IValidate, IId
	{

		public ViewCloudBase()
		{
			// temporary solution for keeping track of clouds in a study
			viewID = Guid.NewGuid().ToString();
		}

		[DetachProperty]
		public Pointcloud cloud { get; set; }

		[DetachProperty]
		public List<string> cloudMetaData { get; set; }

		[JsonIgnore]
		public int count => cloud?.points?.Count / 3 ?? 0;

		[JsonIgnore]
		public List<CloudPointBase> points
		{
			get =>
				cloud?.GetPoints().Select(p => new CloudPointBase
				{
					x = p.x, y = p.y, z = p.z, bbox = p.bbox
				}).ToList();
			set
			{
				var cp = new List<double>();
				cloudMetaData = new List<string>();
				foreach (var point in value)
				{
					cp.Add(point.x);
					cp.Add(point.y);
					cp.Add(point.z);
					cloudMetaData.Add(point.meta);
				}

				if (cloud == null)
					cloud = new Pointcloud();

				cloud.points = cp;
			}
		}

		[JsonIgnore]
		public virtual bool isValid => cloud != null;

		public string viewID { get; set; }
	}
}