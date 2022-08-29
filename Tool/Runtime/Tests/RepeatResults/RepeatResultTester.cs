#region

using System;
using System.Collections.Generic;
using ViewObjects;
using ViewObjects.Unity;

#endregion

namespace ViewTo.Connector.Unity
{

	public class RepeatResultTester
	{

		ResultCloudMono testCloudA, testCloudB;
		ResultsForCloud testResultsA, testResultsB;

		public void SetNewCloud(ResultCloudMono mono)
		{
			if (testCloudA == null)
			{
				testCloudA = mono;
				return;
			}

			if (testCloudB == null) testCloudB = mono;

			if (testCloudA != null && testCloudB != null)
			{
				ViewConsole.Log("ready to compare");

				Compare(testCloudA, testCloudB);
			}
		}

		public void SetNewCloud(ResultsForCloud res)
		{
			if (testResultsA == null)
			{
				testResultsA = res;
				return;
			}

			testResultsB ??= res;

			if (testResultsA != null && testResultsB != null)
			{
				ViewConsole.Log("ready to compare");

				Compare(testResultsA, testResultsB);
			}
		}

		static void Compare(ResultsForCloud cloudA, ResultsForCloud cloudB)
		{
			ViewConsole.Log("Comparing Data!");

			if (cloudA == null || cloudB == null)
			{
				ViewConsole.Error("Clouds are not ready to compare");
				return;
			}

			// Test against data count
			if (cloudA.data.Count != cloudB.data.Count)
			{
				ViewConsole.Error($"Cloud data count are not equal. A={cloudA.data.Count} : B={cloudB.data.Count} ");
				return;
			}

			var valuesA = new Dictionary<ResultType, List<double>>();
			var valuesB = new Dictionary<ResultType, List<double>>();

			// Test all data now
			for (var dIndex = 0; dIndex < cloudA.data.Count; dIndex++)
			{
				// grab collection of data
				var dataA = cloudA.data[dIndex];
				var dataB = cloudB.data[dIndex];

				valuesA.Add(Enum.Parse<ResultType>(dataA.stage), dataA.values);
				valuesB.Add(Enum.Parse<ResultType>(dataB.stage), dataB.values);

				// Test compare target count
				if (dataA.values.Count != dataB.values.Count)
				{
					ViewConsole.Error($"Data sample {dIndex} does not have equal collections. A={dataA.values.Count} : B={dataB.values.Count} ");
					continue;
				}

				ViewConsole.Log($"Testing collection {dIndex}");
				// collect any indexes that do not equal each other
				var mismatchedValues = new List<int>();

				for (var rIndex = 0; rIndex < dataA.values.Count; rIndex++)
				{
					var valueA = dataA.values[rIndex];
					var valueB = dataB.values[rIndex];

					// Test compare target count
					if (!valueA.Equals(valueB))
					{
						mismatchedValues.Add(rIndex);
						ViewConsole.Error($"Values at {rIndex} do not align. A={valueA} : B={valueB} ");
					}
				}

				ViewConsole.Log(mismatchedValues.Valid() ? $"Incorrect values found {mismatchedValues.Count}" : "All values equal");
			}

			var setA = new TestResultContainer();

			foreach (var a in valuesA)
				setA.Set(a.Key.ToString(), a.Value);

			var setB = new TestResultContainer();

			foreach (var a in valuesB)
				setB.Set(a.Key.ToString(), a.Value);

			ViewConsole.Log("Checking Set-A Values");
			CheckSet(setA);

			ViewConsole.Log("Checking Set-B Values");
			CheckSet(setB);
		}

		static void CheckSet(TestResultContainer set)
		{
			var concerns_prop = new List<int>();
			var concerns_existing = new List<int>();

			for (var i = 0; i < set.potential.Count; i++)
			{
				var potential = set.potential[i];
				var existing = set.existing[i];
				var proposed = set.proposed[i];

				if (potential < existing)
				{
					ViewConsole.Warn($"Issue with values! {nameof(potential)}-{potential} vs {nameof(existing)}-{existing}}}");
					concerns_prop.Add(i);
				}

				if (existing < proposed)
				{
					ViewConsole.Warn($"Issue with values! {nameof(existing)}-{existing} vs {nameof(proposed)}-{proposed}}}");
					concerns_existing.Add(i);
				}
			}

			ViewConsole.Log(concerns_prop.Valid() ? $"Concerned Values for Potential vs Existing Found {concerns_prop.Count}" : "Values seem good!");
			ViewConsole.Log(concerns_existing.Valid() ? $"Concerned Values for Existing vs Proposed Found {concerns_existing.Count}" : "Values seem good!");
		}

		static void Compare(ResultCloudMono cloudA, ResultCloudMono cloudB)
		{
			ViewConsole.Log("Comparing Data!");

			if (cloudA == null || cloudB == null)
			{
				ViewConsole.Error("Clouds are not ready to compare");
				return;
			}

			// Test against point count
			if (cloudA.count != cloudB.count)
			{
				ViewConsole.Error($"Cloud points are not equal. A={cloudA.count} : B={cloudB.count} ");
				return;
			}

			// Test against data count
			if (cloudA.data.Count != cloudB.data.Count)
			{
				ViewConsole.Error($"Cloud data count are not equal. A={cloudA.data.Count} : B={cloudB.data.Count} ");
				return;
			}

			// Test compare target count
			if (cloudA.targets.Count != cloudB.targets.Count)
			{
				ViewConsole.Error($"Cloud Target count are not equal. A={cloudA.targets.Count} : B={cloudB.targets.Count} ");
				return;
			}

			// Test all data now
			for (var dIndex = 0; dIndex < cloudA.data.Count; dIndex++)
			{
				// grab collection of data
				var dataA = cloudA.data[dIndex];
				var dataB = cloudB.data[dIndex];

				// Test compare target count
				if (dataA.values.Count != dataB.values.Count)
				{
					ViewConsole.Error($"Data sample {dIndex} does not have equal collections. A={dataA.values.Count} : B={dataB.values.Count} ");
					continue;
				}

				ViewConsole.Log($"Testing collection {dIndex}");
				// collect any indexes that do not equal each other
				var mismatchedValues = new List<int>();

				for (var rIndex = 0; rIndex < dataA.values.Count; rIndex++)
				{
					var valueA = dataA.values[rIndex];
					var valueB = dataB.values[rIndex];

					// Test compare target count
					if (!valueA.Equals(valueB))
					{
						mismatchedValues.Add(rIndex);
						ViewConsole.Error($"Values at {rIndex} do not align. A={valueA} : B={valueB} ");
					}
				}

				ViewConsole.Log(mismatchedValues.Valid() ? $"Incorrect values found {mismatchedValues.Count}" : "No Incorrect values found");
			}
		}

		[Serializable]
		public class TestResultContainer
		{

			public List<double> potential = new();
			public List<double> existing = new();
			public List<double> proposed = new();

			public TestResultContainer()
			{ }

			public TestResultContainer(ResultsForCloud cloud)
			{
				foreach (var container in cloud.data)
					Set(container.stage, container.values);
			}

			public void Set(string key, List<double> values)
			{
				var res = Enum.Parse<ResultType>(key);

				switch (res)
				{
					case ResultType.Potential:
						potential = values;
						break;
					case ResultType.Existing:
						existing = values;
						break;
					case ResultType.Proposed:
						proposed = values;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
	}
}