using System;
using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewObjects.Explorer;

namespace ViewTo
{
	public static partial class Commander
	{

		#region filtering commands

		/// <summary>
		/// Grab a collection of values from the explorer.
		/// </summary>
		/// <param name="explorer"></param>
		/// <param name="stage">The type of values to fetch for</param>
		/// <returns></returns>
		public static IEnumerable<double> Fetch(this IResultExplorer explorer, ResultStage stage)
		{
			var data = new List<double>();

			explorer.TryGetValues(stage, ref data);

			return data;
		}

		public static void SetActiveValues(this ResultExplorer explorer, ResultStage stage, string target = null)
		{
			if (target.Valid() && explorer.targets.Contains(target) && !explorer.activeTarget.Valid() || !explorer.activeTarget.Equals(target))
				explorer.activeTarget = target;

			if (explorer.ActiveStage != stage)
				explorer.ActiveStage = stage;

			var values = new List<double>();

			if (!explorer.TryGetValues(explorer.ActiveStage, ref values)) return;

			explorer.activeValues = values.ToArray();
		}

		public static void SetActiveValues(this ResultExplorer explorer, ResultStage stage, bool normalize, string target = null)
		{
			if (target.Valid() && explorer.targets.Contains(target) && !explorer.activeTarget.Valid() || !explorer.activeTarget.Equals(target))
				explorer.activeTarget = target;

			if (explorer.ActiveStage != stage)
				explorer.ActiveStage = stage;

			var values = new List<double>();
			if (!explorer.TryGetValues(explorer.ActiveStage, ref values))
				return;

			if (!normalize)
			{
				explorer.activeValues = values.ToArray();
				return;
			}

			values.GetMaxMin(out var max, out var min);

			explorer.activeValues = values.NormalizeValues(max, min).ToArray();
		}

		public static IEnumerable<double> GetExistingOverPotential(this IResultExplorer explorer) =>
			explorer.GetComparedValues(ResultStage.Existing, ResultStage.Potential);

		public static IEnumerable<double> GetProposedOverExisting(this IResultExplorer explorer) =>
			explorer.GetComparedValues(ResultStage.Proposed, ResultStage.Existing);

		public static IEnumerable<double> GetProposedOverPotential(this IResultExplorer explorer) =>
			explorer.GetComparedValues(ResultStage.Proposed, ResultStage.Potential);

		public static IEnumerable<double> GetComparedValues(this IResultExplorer explorer, ResultStage stageA, ResultStage stageB)
		{
			var dataA = new List<double>();
			var dataB = new List<double>();

			if (explorer.TryGetValues(stageA, ref dataA))
				if (explorer.TryGetValues(stageB, ref dataB))
					return dataA.NormalizeValues(dataB);

			return null;
		}

		#endregion

		public static bool TryGet(this IResultExplorer explorer, ExplorerValueType valueType, List<string> targets, out IEnumerable<double> results)
		{
			results = new List<double>();

			if (!targets.Valid() || !explorer.HasTargets(targets))
			{
				Console.WriteLine("No Valid Targets to look for ");
				return false;
			}

			valueType.GetStages(out var stageA, out var stageB);

			var compiledStageA = new List<double>();
			var compiledStageB = new List<double>();

			foreach (var t in targets)
			{
				// set the active target
				explorer.activeTarget = t;

				var dataStageA = new List<double>();
				var dataStageB = new List<double>();

				if (!explorer.TryGetValues(stageA, ref dataStageA) || !explorer.TryGetValues(stageB, ref dataStageB))
					return false;

				// if this is the first stage
				if (compiledStageA.Count == 0)
				{
					compiledStageA.AddRange(dataStageA);
					compiledStageB.AddRange(dataStageB);
				}
				else
				{
					for (int i = 0; i < dataStageA.Count; i++)
					{
						compiledStageA[i] += dataStageA[i];
						compiledStageB[i] += dataStageB[i];
					}
				}
			}

			results = compiledStageA.NormalizeValues(compiledStageB);

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

		public static bool TryGetValues(this IResultExplorer explorer, ResultStage stage, ref List<double> data)
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

		public static bool DataIsReady(this IResultExplorer exp)
		{
			return exp.storedData.Valid() && exp.activeTarget.Valid();
		}

		/// <summary>
		/// uses active values
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="valueToFind"></param>
		/// <returns></returns>
		public static int FindPointWithValue(this IResultExplorer obj, double valueToFind) => obj.activeValues.FindPointWithValue(valueToFind);

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
	}
}