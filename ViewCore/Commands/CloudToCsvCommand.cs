using System;
using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewTo.Events.Prime;

namespace ViewTo.Cmd
{
	internal class CloudToCsvCommand : ICommandWithResult<CsvDataArgs>
	{

		protected IResultCloudV1 CloudV1;

		public CloudToCsvCommand(IResultCloudV1 cloudV1) => this.CloudV1 = cloudV1;

		protected IEnumerable<string> VectorHeader
		{
			get =>
				new[]
				{
					"x", "y", "z", "xn", "yn", "zn", "meta"
				};
		}

		public CsvDataArgs args { get; private set; }

		public void Run()
		{
			if (CloudV1 == null || !CloudV1.data.Valid() || !CloudV1.points.Valid())
			{
				args = new CsvDataArgs(null);
				return;
			}

			var lines = new List<string>();

			// add content types to header
			var headers = VectorHeader.ToList();
			for (var i = 0; i < CloudV1.data.Count; i++)
				headers.Add(CloudV1.data[i].stage + "-" + CloudV1.data[i].content + (i == CloudV1.data.Count - 1 ? "," : ""));

			lines.Add(Join(headers));

			for (var pointIndex = 0; pointIndex < CloudV1.points.Length; pointIndex++)
			{
				var line = new List<string>
				{
					PointToLine(CloudV1.points[pointIndex])
				};

				foreach (var data in CloudV1.data)
					line.Add(data.values[pointIndex].ToString());

				lines.Add(Join(line));
			}

			args = new CsvDataArgs(string.Join(Environment.NewLine, lines));
		}

		protected static string Join(IEnumerable<string> values) => string.Join(",", values);

		public static string PointToLine(CloudPoint p)
		{
			return Join(
				new[]
				{
					p.x.ToString(), p.y.ToString(), p.z.ToString(), p.xn.ToString(), p.yn.ToString(), p.zn.ToString(), p.meta
				});
		}

		public static CloudPoint LineToPoint(string lineData)
		{
			var row = lineData.Split(',');
			float.TryParse(row[Columns.X], out var x);
			float.TryParse(row[Columns.Y], out var y);
			float.TryParse(row[Columns.Z], out var z);
			float.TryParse(row[Columns.XNormal], out var xn);
			float.TryParse(row[Columns.YNormal], out var yn);
			float.TryParse(row[Columns.ZNormal], out var zn);
			return new CloudPoint(x, y, z, xn, yn, zn);
		}

		protected readonly struct Columns
		{

			public const int X = 0;
			public const int Y = X + 1;
			public const int Z = Y + 1;
			public const int XNormal = Z + 1;
			public const int YNormal = XNormal + 1;
			public const int ZNormal = YNormal + 1;
			public const int FirstResult = ZNormal + 1;

		}

		protected readonly struct Rows
		{

			public const int NameId = 0;
			public const int Header = NameId + 1;
			public const int FirstPoint = Header + 1;
			public const int NonPointCount = 2;
		}
	}
}