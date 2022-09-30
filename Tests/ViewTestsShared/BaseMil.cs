using System;
using System.Collections.Generic;

namespace ViewTo.Tests.Integration
{
	public static class BaseMil
	{

		public const string TAR_A = "TargetA";
		public const string TAR_B = "TargetB";
		public const string OPT_A = "DesignA";
		public const string OPT_B = "DesignB";

		public static List<uint> ValuesUint(int valueCount, Random rnd = null)
		{
			rnd ??= new Random();
			var values = new List<uint>();

			for (var j = 0; j < valueCount; j++)
				values.Add((uint)rnd.Next());
			return values;
		}

		public static List<int> ValuesInt(int valueCount, Random rnd = null)
		{
			rnd ??= new Random();
			var values = new List<int>();

			for (var j = 0; j < valueCount; j++)
				values.Add(rnd.Next());
			return values;
		}

		public static List<double> ValuesDouble(int valueCount, Random rnd = null)
		{
			rnd ??= new Random();
			var values = new List<double>();

			for (var j = 0; j < valueCount; j++)
				values.Add(rnd.NextDouble());
			return values;
		}
	}
}