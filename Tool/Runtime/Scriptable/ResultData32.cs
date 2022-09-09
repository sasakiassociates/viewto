using System;
using Sasaki;
using UnityEngine;
using ViewObjects;

namespace ViewTo.Connector.Unity
{

	[Serializable]
	public struct ResultData32
	{
		public Color32 color;
		public string meta;
		public RigStage stage;

		public int[] values;

		public ResultData32(int[] values, Color32 color, RigStage stage, string meta = null)
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