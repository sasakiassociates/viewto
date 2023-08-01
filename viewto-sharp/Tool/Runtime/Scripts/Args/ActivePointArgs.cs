#region

using System;
using UnityEngine;

#endregion

namespace ViewTo.Connector.Unity.Commands
{

	public class AReportEventArgs : EventArgs
	{
		public  string message;

	}

	public class ActivePointArgs : AReportEventArgs
	{
		public readonly Vector3 center;

		public readonly Vector3 position;

		public ActivePointArgs(Vector3 position, Vector3 center)
		{
			this.center = center;
			this.position = position;

			message = $"ViewerRig Position {position} Center {center}";
		}
	}
}