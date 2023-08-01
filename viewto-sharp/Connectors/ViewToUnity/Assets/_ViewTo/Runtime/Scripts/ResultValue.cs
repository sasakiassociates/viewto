#region

using System;

#endregion

namespace ViewTo.Connector.Unity
{
	[Serializable]
	public struct ProposedValue
	{

		public float value;
		public string name;

		public ProposedValue(string name, float value = 0)
		{
			this.value = value;
			this.name = name;
		}
	}

	[Serializable]
	public struct ResultValue
	{
		public float potential;

		public float existing;

		public float proposed;

		// public Dictionary<string, float> proposed;
	}
}