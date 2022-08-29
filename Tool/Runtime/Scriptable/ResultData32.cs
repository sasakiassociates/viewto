using System;
using Sasaki;
using UnityEngine;
using ViewObjects;

namespace ViewTo.Connector.Unity
{

	[Serializable]
	public readonly struct ResultData32
	{
		public readonly Color32 color;
		public readonly string meta;
		public readonly RigStage stage;

		public readonly double[] values;

		public ResultData32(double[] values, Color32 color, RigStage stage, string meta = null)
		{
			this.values = values;
			this.color = color;
			this.stage = stage;
			this.meta = meta;
		}

		public ResultData32(IResultData data)
		{
			values = data.values.ToArray();
			color = data.color.ToUnityColor32();
			stage = (RigStage)Enum.Parse(typeof(RigStage), data.stage);
			meta = data.meta;
		}
	}

}