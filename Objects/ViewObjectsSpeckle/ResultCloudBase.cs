using System.Collections.Generic;
using System.Text.Json.Serialization;
using Speckle.Core.Kits;
using Speckle.Core.Logging;
using Speckle.Core.Models;

namespace ViewObjects.Speckle
{
	/// <summary>
	/// </summary>
	public class ResultCloudBase : ViewObjectBase, IResultCloud_v2
	{

		/// <summary>
		/// </summary>
		public ResultCloudBase()
		{ }

		/// <summary>
		///   Constructs a Result cloud
		/// </summary>
		/// <param name="points"></param>
		/// <param name="data"></param>
		/// <param name="viewId"></param>
		[SchemaInfo("Result Cloud", "A view analysis cloud with result data attached", ViewObjectSpeckle.Schema.Category, "Objects")]
		public ResultCloudBase(CloudPoint[] points, List<IResultCloudData> data, string viewId = null)
		{
			Data = data;
			Points = points;
			ViewId = ObjUtils.CheckIfValidId(viewId);
		}

		/// <summary>
		///   List of point positions as x,y,z
		///   This should be set using <see cref="Points" />
		/// </summary>
		[DetachProperty] [Chunkable(31250)] public List<double> Positions { get; set; } = new List<double>();

		/// <summary>
		///   List of meta data for <see cref="CloudPoint" />
		///   This should be set using <see cref="Points" />
		/// </summary>
		[DetachProperty] [Chunkable(31250)] public List<string> MetaData { get; set; } = new List<string>();

		/// <inheritdoc />
		public string ViewId { get; set; }

		/// <inheritdoc />
		public List<IResultCloudData> Data { get; set; } = new List<IResultCloudData>();

		/// <inheritdoc />
		[JsonIgnore] public CloudPoint[] Points
		{
			get
			{
				if (Positions.Count % 3 != 0)
					throw new SpeckleException($"{nameof(ResultCloudBase)}.{nameof(Positions)} list is malformed: expected length to be multiple of 3");

				var points = new CloudPoint[Positions.Count / 3];

				for (int i = 2, c = 0; i < Positions.Count; i += 3, c++)
					points[i] = new CloudPoint(Positions[i - 2], Positions[i - 1], Positions[i], 0, 0, 0, MetaData[c] ?? "empty");

				return points;
			}
			set
			{
				Positions = new List<double>();
				MetaData = new List<string>();

				foreach (var point in value)
				{
					Positions.Add(point.x);
					Positions.Add(point.y);
					Positions.Add(point.z);
					MetaData.Add(point.meta);
				}
			}
		}
	}
}