#region

using System;

#endregion

namespace ViewTo.Connector.Unity.Tests
{

	[Serializable]
	public class DebugValues
	{
		public double potential;

		public double existing;

		public double proposed;
	}

	[Serializable]
	public class LayoutDebuggerValues : DebugValues
	{
		public ViewerBundleSystem sys { get; set; }
	}
}