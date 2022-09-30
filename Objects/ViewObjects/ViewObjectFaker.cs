using System;
using System.Collections.Generic;
using System.Linq;
using ViewObjects.Viewer;

namespace ViewObjects
{
	public static class ViewObjectFaker
	{
		public static TCloud ResultCloud<TCloud>(int pointCount, int colorCount)
			where TCloud : IResultCloud
		{
			var obj = Activator.CreateInstance<TCloud>();
			obj.Points = CloudPoints(pointCount);
			obj.Data = Results<ResultCloudData>(pointCount, colorCount).Cast<IResultCloudData>().ToList();
			return obj;
		}

		public static TCloud ResultCloud<TCloud, TData>(int pointCount, int colorCount)
			where TCloud : IResultCloud<TData> where TData : IResultCloudData
		{
			var obj = Activator.CreateInstance<TCloud>();
			obj.Points = CloudPoints(pointCount);
			obj.Data = Results<TData>(pointCount, colorCount);
			return obj;
		}

		public static List<TData> Results<TData>(int pointCount, int colorCount) where TData : IResultCloudData
		{
			var values = new List<TData>();
			var random = new Random();

			for (int c = 0; c < colorCount; c++)
			{
				var id = ObjUtils.InitGuid;
				foreach (ResultStage stage in Enum.GetValues(typeof(ResultStage)))
				{
					values.Add(Result<TData>(pointCount, stage, id, nameof(ViewerLayout), $"Test{c}", random));
				}
			}

			return values;
		}

		public static TData Result<TData>(
			int pointCount,
			ResultStage stage,
			string id = null,
			string layout = null,
			string contentName = null,
			Random random = null
		) where TData : IResultCloudData
		{
			random ??= new Random();
			var obj = Activator.CreateInstance<TData>();
			obj.Layout = string.IsNullOrEmpty(layout) ? nameof(ViewerLayout) : layout;
			obj.Values = Values(pointCount, random);
			obj.Option = ContentOption(
				name: string.IsNullOrEmpty(contentName) ? "Test" : contentName,
				id: ObjUtils.CheckIfValidId(id),
				stage: stage);
			return obj;
		}

		public static IContentOption ContentOption(string name = "test", string id = null, ResultStage stage = ResultStage.Existing)
		{
			return new ContentOption
			{
				Id = ObjUtils.CheckIfValidId(id), Stage = stage, Name = name
			};
		}

		public static CloudPoint[] CloudPoints(int count)
		{
			var rnd = new Random();
			var points = new CloudPoint[count];
			for (var i = 0; i < points.Length; i++)
				points[i] = new CloudPoint(rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble(),
				                           rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble(), "1234-567-890");
			return points;
		}

		public static List<int> Values(int valueCount, Random rnd = null)
		{
			rnd ??= new Random();

			var values = new List<int>();
			for (var j = 0; j < valueCount; j++)
			{
				var bytes = new byte[4];
				rnd.NextBytes(bytes);
				values.Add((int)BitConverter.ToUInt32(bytes, 0));
			}

			return values;
		}
	}
}