using System;
using System.Collections.Generic;

namespace ViewTo.Receivers
{
	public class ValueReceiver
	{
		//https://floating-point-gui.de/errors/comparison/
		internal const double DBL_EPSILON = 2.22044604925031E-16;

		//https://stackoverflow.com/questions/3874627/floating-point-comparison-functions-for-c-sharp
		public bool NearlyEqual(double value1, double value2, double unimportantDifference = 0.0001)
		{
			if (double.IsNaN(value1) || double.IsNaN(value2))
			{
				return false;
			}

			return Math.Abs(value1 - value2) < unimportantDifference;
		}

		public double[] PowLog(double[] values, double maxValue, double multiplier, double maxScore, double minValue = 0.0000001)
		{
			var maxCount = maxValue * multiplier;
			var logCustomBase = Math.Pow(maxCount, 1 / maxScore);

			var outputValues = new double[values.Length];

			for (var i = 0; i < values.Length; i++)
			{
				var pxValue = values[i];

				outputValues[i] = !double.IsNaN(pxValue) && pxValue > minValue ? Math.Log(pxValue * multiplier, logCustomBase) : pxValue;
			}

			return outputValues;
		}

		public double[] Log(IReadOnlyList<int> values, int maxValue, double desiredScore = 1)
		{
			var logCustomBase = Math.Pow(maxValue, 1 / desiredScore);
			var outputValues = new double[values.Count];

			for (var i = 0; i < values.Count; i++)
			{
				outputValues[i] = Math.Log(values[i], logCustomBase);
			}

			return outputValues;
		}
	}
}