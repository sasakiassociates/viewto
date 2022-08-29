#region

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace ViewTo.Connector.Unity
{

	public class SoLayoutSystemData : ScriptableObject
	{
		[SerializeField]
		Color32[] analysisColors;
		[SerializeField]
		List<ResultData32> results;

		public double[][] data { get; private set; }

		public void Add(double[] values, int index)
		{
			Debug.Log($"adding to {index}");
			data[index] = values;
		}

		public void Create(int pointCount, Color32[] colors)
		{
			data = new double[pointCount][];
			analysisColors = colors;
		}

		public List<ResultData32> GetAll() => results;

		public void Compile(RigStage rigStage, string meta = null)
		{
			Debug.Log($"Compiling {name} to {rigStage}");
			results ??= new List<ResultData32>();

			for (var colorIndex = 0; colorIndex < analysisColors.Length; colorIndex++)
			{
				var colorValuesByPoint = new double[data.Length];
				// get values of color from each point
				for (var pointIndex = 0; pointIndex < data.Length; pointIndex++)
				{
					var value = data[pointIndex][colorIndex];

					colorValuesByPoint[pointIndex] = value;

					if (pointIndex == ViewToUtils.Point9911.index) ViewToUtils.Point9911.Compare(rigStage, value);
				}

				results.Add(new ResultData32(
					            colorValuesByPoint,
					            analysisColors[colorIndex],
					            rigStage, meta));
			}
		}
	}
}