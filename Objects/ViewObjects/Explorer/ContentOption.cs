using System;

namespace ViewObjects.Explorer
{
	[Serializable]
	public struct ContentOption
	{
		/// <summary>
		/// the name of the active target to use 
		/// </summary>
		public string target;

		/// <summary>
		/// the stage to use for
		/// </summary>
		public ResultStage stage;
	}
}