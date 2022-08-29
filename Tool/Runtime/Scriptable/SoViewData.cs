#region

using UnityEngine;

#endregion

namespace ViewTo.Connector.Unity
{

	public class SoViewData : ScriptableObject
	{
		[SerializeField]
		int contentCount;
		[SerializeField]
		double[] pixelData;

		public double[] GetPixelData
		{
			get => pixelData;
		}

		public void Create(int contentColors)
		{
			contentCount = contentColors;
			pixelData = new double[contentCount];
		}

		public void Add(double[] values)
		{
			pixelData = values;
		}
	}
}