#region

using System;
using System.Collections.Generic;
using ViewObjects;
using Random = UnityEngine.Random;

#endregion

namespace ViewTo.Connector.Unity
{
	public static class ViewToCoreExtensions
	{
		internal const double DBL_EPSILON = 2.22044604925031E-16;
		//https://floating-point-gui.de/errors/comparison/

		//https://stackoverflow.com/questions/3874627/floating-point-comparison-functions-for-c-sharp
		public static bool NearlyEqual(this double value1, double value2, double unimportantDifference = 0.0001)
		{
			if (double.IsNaN(value1) || double.IsNaN(value2))
				return false;

			return Math.Abs(value1 - value2) < unimportantDifference;
		}

		public static int CheckCollection(List<double> values, double valueToFind)
		{
			var res = -1;

			if (double.IsNaN(valueToFind))
			{
				ViewConsole.Warn("Invalid value to find");
				return res;
			}

			if (!values.Valid())
			{
				ViewConsole.Warn("No stored data available");
				return res;
			}

			var sampleOfValues = new List<int>();

			// compare data 
			for (var i = 0; i < values.Count; i++)
				if (values[i].NearlyEqual(valueToFind))
					sampleOfValues.Add(i);

			// if no values were found from sample set we keep searching
			if (!sampleOfValues.Valid())
			{
				ViewConsole.Log($"No values found that match {valueToFind}");

				// if no values were found we look for the nearest values
				double nearest = 1;

				// TODO: Look into how this is handling the rounding
				for (var i = 0; i < values.Count; i++)
				{
					var diff = Math.Abs(values[i] - valueToFind);
					if (diff < nearest)
					{
						nearest = diff;
						res = i;
					}
				}

				ViewConsole.Log($"Nearest value found is {values[res]} at {res}");

				return res;
			}

			// return a random index from sample 
			Random.InitState(DateTime.Now.Millisecond);
			res = sampleOfValues[Random.Range(0, sampleOfValues.Count - 1)];

			ViewConsole.Log($"index set to {res} out of {sampleOfValues.Count}");
			return res;
		}

		public static int FindPointWithValue(this IResultExplorer obj, double valueToFind, ResultStage ResultStage = default, string target = null)
		{
			if (!target.Valid())
				target = obj.activeTarget;
			if (ResultStage == default)
				ResultStage = obj.activeStage;

			var res = 0;
			// foreach (var resultData in obj.storedData)
			// 	if (resultData.content.Valid()
			// 	    && resultData.content.Equals(target)
			// 	    && resultData.stage.Valid()
			// 	    && ResultStage.CheckAgainstString(resultData.stage))
			// 		res = CheckCollection(resultData.values, valueToFind);

			return res;
		}

		public static int FindBest(this IResultExplorer obj)
		{
			if (!obj.activeValues.Valid())
			{
				ViewConsole.Warn("No active values stored");
				return-1;
			}

			var tempMax = 0.0;
			var maxIndex = 0;

			for (var i = 0; i < obj.activeValues.Length; i++)
			{
				var value = obj.activeValues[i];

				if (value <= tempMax) continue;

				tempMax = value;
				maxIndex = i;
			}

			return maxIndex;
		}
	}
}