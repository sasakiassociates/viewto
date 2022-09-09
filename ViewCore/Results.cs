using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ViewObjects;
using ViewObjects.Cloud;
using ViewObjects.Content;

namespace ViewTo
{

	public static class Results
	{

		public static bool CheckAgainstString(this ResultStage stage, string type)
		{
			if (!type.Valid())
				return false;

			// previous version of organizing data types
			var nameToCompare = stage switch
			{
				ResultStage.Potential => "Target",
				ResultStage.Existing => "Blocker",
				ResultStage.Proposed => "Design",
				_ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
			};

			var res = type.ToUpper().Equals(nameToCompare.ToUpper());

			if (res)
				return true;

			// try other version
			nameToCompare = stage switch
			{
				ResultStage.Potential => "Potential",
				ResultStage.Existing => "Existing",
				ResultStage.Proposed => "Proposed",
				_ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
			};

			return type.ToUpper().Equals(nameToCompare.ToUpper());
		}

		public static List<string> GetTargets(this IResultCloud obj)
		{
			// using a hash should not allow for duplicated target names
			var targetNames = new HashSet<string>();

			if (obj != default)
			{
				foreach (var data in obj.data)
				{
					if (data == null)
						continue;

					if (data.content.Valid())
						targetNames.Add(data.content);
					else
						Console.WriteLine($"MISSING TARGET FOR {data}");
				}
			}

			return targetNames.ToList();
		}

		public static bool Check(this IId obj, IId input)
		{
			return obj != default && obj.viewID.Valid() && input != default && input.viewID.Valid() && obj.viewID.Equals(input.viewID);
		}

		public static ResultCloud GetResults(string path)
		{
			if (!File.Exists(path))
				return null;

			var rc = new ResultCloud
			{
				data = new List<IResultData>()
			};

			var lines = File.ReadAllLines(path);

			var header = lines[0].Split(',');

			var points = new List<CloudPoint>();
			for (var i = 1; i < lines.Length; i++)
			{
				var line = lines[i].Split(',');
				points.Add(new CloudPoint(
					           double.Parse(line[0]), double.Parse(line[1]), double.Parse(line[2]))
				);
			}

			rc.points = points.ToArray();

			for (var colIndex = 7; colIndex < header.Length; colIndex++)
			{
				var contentName = header[colIndex].Split('-').First();
				var stageName = header[colIndex].Split('-').Last();

				var values = new List<int>();
				for (var i = 1; i < lines.Length; i++)
					values.Add(int.Parse(lines[i].Split(',')[colIndex]));

				var d = new ContentResultData(values, stageName, contentName, 0);
				rc.data.Add(d);
			}

			return rc;
		}

		// public static void MapToColor(this IResultCloud obj, IResultData selectedData, Color[] gradient)
		// {
		//   var pts = obj.points;
		//   var values = selectedData.values;
		//
		//   // data and points should match in size
		//   if (pts.Valid(obj.count))
		//     return;
		//
		//   var mappedColors = new List<int>();
		//   foreach (var score in values)
		//     mappedColors.Add(gradient[(int)Math.Round((gradient.Length - 1) * score, 0)].ToArgb());
		//
		//   cloud.colors = mappedColors;
		// }
	}

}