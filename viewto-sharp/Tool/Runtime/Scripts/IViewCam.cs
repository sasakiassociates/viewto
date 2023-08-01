#region

using UnityEngine;

#endregion

namespace ViewTo.Connector.Unity
{
	public interface IViewCam
	{
		public RenderTexture renderText { get; }
	}
}