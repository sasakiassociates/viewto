#region

using UnityEngine;

#endregion

namespace ViewTo.Connector.Unity
{
	[RequireComponent(typeof(Camera))]
	public class MapViewer : MonoBehaviour
	{
		public Camera viewer;

		public void LockPositionToCam(bool b)
		{
			ViewConsole.Log("Tehehe this does nothing!");
		}
	}
}