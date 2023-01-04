#region

using UnityEngine;

#endregion

namespace ViewTo.Connector.Unity
{
	public static class ViewConsole
	{
		const string LABEL = "ViewTo-";

		public static void Error(string message) => Debug.LogError(LABEL + message);

		public static void Log(string message) => Debug.Log(LABEL + message);

		public static void Warn(string message) => Debug.LogWarning(LABEL + message);
	}
}