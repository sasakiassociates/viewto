using System;
using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewObjects.Explorer;
using ViewTo.Values;

namespace ViewTo
{

	public static class ExplorerExt
	{
		/// <summary>
		///   Load in a new cloud for the explorer to explore!
		/// </summary>
		/// <param name="explorer"></param>
		/// <param name="viewObj"></param>
		public static void Load(this IExplorer explorer, IResultCloud viewObj)
		{ }

		/// <summary>
		///   Load in a new view study for the explorer to explore!
		/// </summary>
		/// <param name="explorer"></param>
		/// <param name="viewObj">The view study to load in</param>
		public static void Load(this IExplorer explorer, IViewStudy viewObj)
		{ }

		/// <summary>
		///   Retrieves the active point result data
		/// </summary>
		/// <returns></returns>
		public static ResultPoint GetResultPoint(this IExplorer explorer) => new ResultPoint();
	}

	public static partial class Commander
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

		public static bool TryGet(this IResultExplorer explorer, ExplorerValueType valueType, List<string> targets, out IEnumerable<double> results)
		{
			results = new List<double>();

			return results.Any();
		}

		public static bool TryGet(this IResultExplorer explorer, ExplorerValueType valueType, string target, out ICollection<int> results)
		{
			results = new List<int>();

			if (!target.Valid() || !explorer.HasTarget(target))
			{
				Console.WriteLine("No Valid Targets to look for ");
				return false;
			}

			if (!explorer.CheckActiveTarget(target))
				explorer.activeTarget = target;

			valueType.GetStages(out var stageA, out var stageB);

			results = explorer.GetComparedValuesRaw(stageA, stageB);

			return results.Any();
		}

		public static bool TryGet(this IResultExplorer explorer, ExplorerValueType valueType, string target, out IEnumerable<double> results)
		{
			results = new List<double>();

			if (!target.Valid() || !explorer.HasTarget(target))
			{
				Console.WriteLine("No Valid Targets to look for ");
				return false;
			}

			if (!explorer.CheckActiveTarget(target))
				explorer.activeTarget = target;

			valueType.GetStages(out var stageA, out var stageB);

			results = explorer.GetComparedValues(stageA, stageB);

			return results.Any();
		}

		static void GetStages(this ExplorerValueType type, out ResultStage stageA, out ResultStage stageB)
		{
			switch (type)
			{
				case ExplorerValueType.ExistingOverPotential:
					stageA = ResultStage.Existing;
					stageB = ResultStage.Potential;
					break;
				case ExplorerValueType.ProposedOverExisting:
					stageA = ResultStage.Proposed;
					stageB = ResultStage.Existing;
					break;
				case ExplorerValueType.ProposedOverPotential:
					stageA = ResultStage.Proposed;
					stageB = ResultStage.Potential;
					break;
				default:
					stageA = ResultStage.Potential;
					stageB = ResultStage.Potential;
					break;
			}
		}

		public static bool TryGetValues(this IResultExplorer explorer, ResultStage stage, ref List<int> data)
		{
			try
			{
				// select pixel data with target 
				foreach (var d in explorer.storedData)
				{
					if (!d.content.Equals(explorer.activeTarget))
						continue;

					if (stage.CheckAgainstString(d.stage))
					{
						data = d.values;
						break;
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}

			return data != null && data.Any();
		}

		public static bool CheckActiveTarget(this IResultExplorer explorer, string target) => target.Valid() && target.Equals(explorer.activeTarget);

		public static bool HasTargets(this IResultExplorer explorer, List<string> targets)
		{
			if (!targets.Valid())
				return false;

			foreach (var t in targets)
				if (!explorer.HasTarget(t))
					return false;

			return true;
		}

		public static bool HasTarget(this IResultExplorer explorer, string target) =>
			explorer.targets.Valid() && explorer.targets.Any(x => x.Valid() && x.Equals(target));

		public static bool DataIsReady(this IResultExplorer exp) => exp.storedData.Valid() && exp.activeTarget.Valid();
		//
		// /// <summary>
		// /// uses active values
		// /// </summary>
		// /// <param name="obj"></param>
		// /// <param name="valueToFind"></param>
		// /// <returns></returns>
		// public static int FindPointWithValue(this IResultExplorer obj, double valueToFind) => obj.activeValues.FindPointWithValue(valueToFind);

		/// <summary>
		///   uses active values
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="valueToFind"></param>
		/// <returns></returns>
		public static int FindPointWithValue(this IResultExplorer obj, int valueToFind) => obj.activeValues.FindPointWithValue(valueToFind);

		public static int FindPointWithValue(this int[] values, int valueToFind)
		{
			var res = -1;

			if (double.IsNaN(valueToFind) || !values.Valid())
				return res;

			var sampleOfValues = new List<int>();

			// compare data 
			for (var i = 0; i < values.Length; i++)
				if (values[i].Equals(valueToFind))
					sampleOfValues.Add(i);

			// if no values were found from sample set we keep searching
			if (!sampleOfValues.Valid())
			{
				// if no values were found we look for the nearest values
				var nearest = 1.0;

				for (var i = 0; i < values.Length; i++)
				{
					var diff = Math.Abs(values[i] - valueToFind);
					if (diff < nearest)
					{
						nearest = diff;
						res = i;
					}
				}

				sampleOfValues.Add(res);
			}

			var r = new Random(DateTime.Now.Millisecond);

			return sampleOfValues[r.Next(0, sampleOfValues.Count - 1)];
		}

		public static int FindPointWithValue(this double[] values, double valueToFind, double unimportantDifference = 0.0001)
		{
			var res = -1;

			if (double.IsNaN(valueToFind) || !values.Valid())
				return res;

			var sampleOfValues = new List<int>();

			// compare data 
			for (var i = 0; i < values.Length; i++)
				if (values[i].NearlyEqual(valueToFind, unimportantDifference))
					sampleOfValues.Add(i);

			// if no values were found from sample set we keep searching
			if (!sampleOfValues.Valid())
			{
				// if no values were found we look for the nearest values
				var nearest = 1.0;

				for (var i = 0; i < values.Length; i++)
				{
					var diff = Math.Abs(values[i] - valueToFind);
					if (diff < nearest)
					{
						nearest = diff;
						res = i;
					}
				}

				sampleOfValues.Add(res);
			}

			var r = new Random(DateTime.Now.Millisecond);

			return sampleOfValues[r.Next(0, sampleOfValues.Count - 1)];
		}

		public static bool InRange(this IExploreRange obj, double value) => value >= obj.min && value <= obj.max;

		#region filtering commands

		/// <summary>
		///   Grab a collection of values from the explorer.
		/// </summary>
		/// <param name="explorer"></param>
		/// <param name="stage">The type of values to fetch for</param>
		/// <returns></returns>
		public static IEnumerable<int> Fetch(this IResultExplorer explorer, ResultStage stage)
		{
			var data = new List<int>();

			explorer.TryGetValues(stage, ref data);

			return data;
		}

		public static void SetActiveValues(this ResultExplorer explorer, ResultStage stage, string target = null)
		{
			if ((target.Valid() && explorer.targets.Contains(target) && !explorer.activeTarget.Valid()) || !explorer.activeTarget.Equals(target))
				explorer.activeTarget = target;

			if (explorer.activeStage != stage)
				explorer.activeStage = stage;

			var values = new List<int>();

			if (!explorer.TryGetValues(explorer.activeStage, ref values))
				return;

			explorer.activeValues = values.ToArray();
		}

		public static void SetActiveValues(this ResultExplorer explorer, ResultStage stage, bool normalize, string target = null)
		{
			if ((target.Valid() && explorer.targets.Contains(target) && !explorer.activeTarget.Valid()) || !explorer.activeTarget.Equals(target))
				explorer.activeTarget = target;

			if (explorer.activeStage != stage)
				explorer.activeStage = stage;

			var values = new List<int>();
			if (!explorer.TryGetValues(explorer.activeStage, ref values))
				return;

			if (!normalize)
			{
				explorer.activeValues = values.ToArray();
				return;
			}

			var math = new IntMath();

			values.GetMaxMin(out var max, out var min);

			explorer.activeValues = new int[values.Count];

			for (var i = 0; i < values.Count; i++)
				explorer.activeValues[i] = math.Normalize(values[i], max, min);
		}

		public static ICollection<int> GetComparedValuesRaw(this IResultExplorer explorer, ResultStage dividendStage, ResultStage divisorStage)
		{
			var dividendValues = new List<int>();
			var divisorValues = new List<int>();
			var result = new List<int>();

			if (explorer.TryGetValues(dividendStage, ref dividendValues) && explorer.TryGetValues(divisorStage, ref divisorValues))
				result = GetValuesCompared(dividendValues, divisorValues, new IntMath()).ToList();

			return result;
		}

		public static void GetMaxMin(this IEnumerable<int> values, int minCap, out int min, out int max)
		{
			min = 0;
			max = 0;

			foreach (var value in values)
			{
				if (value < minCap)
					continue;

				if (value < min)
					min = value;
				if (value > max)
					max = value;
			}
		}

		public static void GetMaxMin(this IEnumerable<double> values, double minCap, out double min, out double max)
		{
			min = 0;
			max = 0;

			foreach (var value in values)
			{
				if (value < minCap)
					continue;

				if (value < min)
					min = value;
				if (value > max)
					max = value;
			}
		}

		/// <summary>
		///   Fetches values from active target in explorer, normalizes the values, and returns the log10 values
		/// </summary>
		/// <param name="explorer"></param>
		/// <param name="dividendStage"></param>
		/// <param name="divisorStage"></param>
		/// <returns></returns>
		public static IEnumerable<double> GetComparedValues(this IResultExplorer explorer, ResultStage dividendStage, ResultStage divisorStage)
		{
			var dividendValues = new List<int>();
			var divisorValues = new List<int>();
			var result = Array.Empty<double>();

			if (explorer.TryGetValues(dividendStage, ref dividendValues) && explorer.TryGetValues(divisorStage, ref divisorValues))
				// var comparedValues = GetValuesCompared(dividendValues, divisorValues, new IntMath()).ToList();
				// comparedValues.GetMaxMin(0, out var min, out var max);
				// result = comparedValues.Log(max, min);
				result = dividendValues.NormalizeValues(divisorValues).ToArray();

			return result;
		}

		/// <summary>
		///   Compares two sets of values and compares them through subtraction.
		/// </summary>
		/// <param name="values"></param>
		/// <param name="baseValues"></param>
		/// <param name="math">basic <see cref="MathProvider{T}" /></param>
		/// <typeparam name="T">The value type to use</typeparam>
		/// <returns></returns>
		public static IEnumerable<T> GetValuesCompared<T>(IReadOnlyList<T> values, IReadOnlyList<T> baseValues, MathProvider<T> math)
		{
			var result = new List<T>();
			for (var i = 0; i < values.Count; i++)
				result.Add(math.Subtract(baseValues[i], values[i]));

			return result;
		}

		#endregion

	}
}