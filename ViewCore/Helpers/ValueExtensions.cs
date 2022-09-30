using System;
using System.Collections.Generic;
using ViewObjects;

namespace ViewTo
{
	public static class ValueExtensions
	{

		//https://floating-point-gui.de/errors/comparison/
		internal const double DBL_EPSILON = 2.22044604925031E-16;

		/// <summary>
		///   Simple command for normalizing view result data.
		/// </summary>
		/// <param name="inputValues">the result type for the dividend value</param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public static IEnumerable<double> NormalizeValues(this List<double> inputValues, double max, double min)
		{
			if (!inputValues.Valid())
				return null;

			var values = new double[inputValues.Count];

			for (var i = 0; i < values.Length; i++)
				values[i] = inputValues[i].NormalizeBy(max, min);

			return values;
		}

		/// <summary>
		///   Simple command for normalizing view result data.
		///   If the divisor value is 0 the value will be set to -1
		/// </summary>
		/// <param name="inputValues">the result type for the dividend value</param>
		/// <param name="divisorValues">the result type for the divisor value</param>
		public static IEnumerable<double> NormalizeValues(this List<double> inputValues, List<double> divisorValues)
		{
			if (!inputValues.Valid() || !divisorValues.Valid() || inputValues.Count != divisorValues.Count)
				return null;

			var values = new double[inputValues.Count];

			for (var i = 0; i < values.Length; i++)
			{
				double value;

				if (divisorValues[i] == 0) // no change in value so set it to -1 as a flag
					value = -1;
				else
					value = inputValues[i].NormalizeBy(divisorValues[i]);

				values[i] = value;
			}

			return values;
		}

		/// <summary>
		///   Simple command for normalizing view result data.
		///   If the divisor value is 0 the value will be set to -1
		/// </summary>
		/// <param name="inputValues">the result type for the dividend value</param>
		/// <param name="divisorValues">the result type for the divisor value</param>
		public static IEnumerable<double> NormalizeValues(this List<int> inputValues, List<int> divisorValues)
		{
			if (!inputValues.Valid() || !divisorValues.Valid() || inputValues.Count != divisorValues.Count)
				return null;

			var values = new double[inputValues.Count];

			for (var i = 0; i < values.Length; i++)
			{
				double value;

				if (divisorValues[i] == 0) // no change in value so set it to -1 as a flag
					value = -1;
				else
					value = inputValues[i].NormalizeBy(divisorValues[i]);

				values[i] = value;
			}

			return values;
		}

		public static void GetMaxMin(this List<int> inputValues, out int max, out int min)
		{
			min = 0;
			max = 0;

			if (!inputValues.Valid())
				return;

			foreach (var value in inputValues)
			{
				if (value <= 0)
					continue;

				if (min > value)
					min = value;
				if (max < value)
					max = value;
			}
		}

		public static void GetMaxMin(this List<uint> inputValues, out uint max, out uint min)
		{
			min = 0;
			max = 0;

			if (!inputValues.Valid())
				return;

			foreach (var value in inputValues)
			{
				if (value <= 0)
					continue;

				if (min > value)
					min = value;
				if (max < value)
					max = value;
			}
		}

		public static void GetMaxMin(this List<double> inputValues, out double max, out double min)
		{
			min = 0;
			max = 0;

			if (!inputValues.Valid())
				return;

			foreach (var value in inputValues)
			{
				if (value <= 0)
					continue;

				if (min > value)
					min = value;
				if (max < value)
					max = value;
			}
		}

		public static void GetMaxMin(this double[] inputValues, out double max, out double min)
		{
			min = 0;
			max = 0;

			if (!inputValues.Valid())
				return;

			foreach (var value in inputValues)
			{
				if (value <= 0)
					continue;

				if (min > value)
					min = value;
				if (max < value)
					max = value;
			}
		}

		public static uint NormalizeBy(this uint value, uint max, uint min = 0) => (value - min) / (max - min);

		public static double NormalizeBy(this int value, double max, double min = 0.0) => (value - min) / (max - min);

		public static float NormalizeBy(this float value, float max, float min = 0.0f) => (value - min) / (max - min);

		public static double NormalizeBy(this double value, double max, double min = 0.0) => (value - min) / (max - min);

		public static float Pow(this float value, double num = 1000) => (float)Math.Pow(value, 1 / num);

		public static double Pow(this double value, double num = 1000) => Math.Pow(value, 1 / num);

		//https://stackoverflow.com/questions/3874627/floating-point-comparison-functions-for-c-sharp
		public static bool NearlyEqual(this double value1, double value2, double unimportantDifference = 0.0001)
		{
			if (double.IsNaN(value1) || double.IsNaN(value2))
				return false;

			return Math.Abs(value1 - value2) < unimportantDifference;
		}

		public static double[] PowLog(this double[] values, double maxValue, double multiplier, double maxScore, double minValue = 0.0000001)
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

		public static double[] Log(this IReadOnlyList<int> values, int maxValue, double desiredScore = 1)
		{
			var logCustomBase = Math.Pow(maxValue, 1 / desiredScore);
			var outputValues = new double[values.Count];

			for (var i = 0; i < values.Count; i++)
				outputValues[i] = Math.Log(values[i], logCustomBase);

			return outputValues;
		}
	}
}