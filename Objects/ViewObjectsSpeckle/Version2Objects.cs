using System.Collections.Generic;
using Speckle.Core.Models;

namespace ViewObjects.Speckle
{

	public class ResultContainer : ViewObjBase
	{
		public ResultContainer()
		{ }

		[DetachProperty, Chunkable]
		public List<double> values
		{
			get;
			set;
		}

		public string content
		{
			get;
			set;
		}
		public ResultType stage
		{
			get;
			set;
		}
		public string meta
		{
			get;
			set;
		}

		public string layout
		{
			get;
			set;
		}
	}
}

// 	public class ViewStudyBase2 : Base, IViewStudy
// 	{
//
// 		public ViewStudyBase2()
// 		{ }
// 		public string viewName { get; set; }
//
// 		[JsonIgnore]
// 		public bool isValid { get; }
//
// 		public List<IViewObj> objs { get; set; }
// 	}
// 	
// 	public class ViewContentBase2 : Base, IViewContent
// 	{
// 		List<object> _objects;
//
// 		public ViewContentBase2()
// 		{ }
//
// 		public ViewColor viewColor { get; set; }
// 		
// 		List<object> IViewContent.objects
// 		{
// 			get => _objects;
// 			set => _objects = value;
// 		}
// 		
// 		[DetachProperty]
// 		public List<Base> objects { get; set; }
//
// 		[JsonIgnore]
// 		public virtual bool isValid => objects != null;
//
// 		public string viewName { get; set; }
// 	}
//
// 	public class ViewCloudBase2 : Pointcloud, IViewCloud
// 	{
//
// 		public ViewCloudBase2()
// 		{
// 			// temporary solution for keeping track of clouds in a study
// 			viewID = Guid.NewGuid().ToString();
// 		}
//
// 		public string viewID { get; set; }
// 		
// 		[DetachProperty]
// 		public List<string> cloudMetaData { get; set; }
//
// 		[JsonIgnore]
// 		public int count => points?.Count / 3 ?? 0;
//
// 		[JsonIgnore]
// 		public virtual bool isValid => points != null;
//
// 		[JsonIgnore]
// 		public CloudPoint[] viewPoints
// 		{
// 			get =>
// 				GetPoints().Select(p => new CloudPoint
// 				{
// 					x = p.x, y = p.y, z = p.z
// 				}).ToArray();
// 			set
// 			{
// 				var cp = new List<double>();
// 				cloudMetaData = new List<string>();
//
// 				foreach (var point in value)
// 				{
// 					cp.Add(point.x);
// 					cp.Add(point.y);
// 					cp.Add(point.z);
// 					cloudMetaData.Add(point.meta);
// 				}
//
// 				points = cp;
// 			}
// 		}
//
//
// 	}
